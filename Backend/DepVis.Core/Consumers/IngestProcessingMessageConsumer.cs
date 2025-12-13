using System.Text.Json;
using DepVis.Core.Context;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Consumers;

public class IngestProcessingMessageConsumer(
    ILogger<IngestProcessingMessageConsumer> _logger,
    DepVisDbContext _db,
    MinioStorageService _minio
) : IConsumer<IngestProcessingMessage>
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public async Task Consume(ConsumeContext<IngestProcessingMessage> context)
    {
        using var _ = _logger.BeginScope(
            new Dictionary<string, object?> { ["SbomId"] = context.Message.SbomId }
        );

        _logger.LogDebug("Starting ingestion.");

        var sbom = await _db
            .Sboms.Include(x => x.ProjectBranch)
            .FirstAsync(x => x.Id == context.Message.SbomId, context.CancellationToken);

        var projectBranch = sbom.ProjectBranch;

        projectBranch.ProcessStep = Shared.Model.Enums.ProcessStep.SbomIngest;
        projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
        await _db.SaveChangesAsync(context.CancellationToken);

        try
        {
            await using var cycloneDxStream = await _minio.RetrieveAsync(
                sbom.FileName,
                context.CancellationToken
            );

            CycloneDxBom bom =
                JsonSerializer.Deserialize<CycloneDxBom>(cycloneDxStream, _jsonOptions)
                ?? new CycloneDxBom { Components = [] };

            try
            {
                await IngestVulnerablities(bom);

                var packages = BuildPackages(sbom.Id, bom);

                var edges = BuildEdges(bom);
                var bomRefToId = packages.ToDictionary(
                    p => p.BomRef,
                    p => p.Id,
                    StringComparer.Ordinal
                );

                var packageVulnerabilities =
                    bom.Vulnerabilities?.SelectMany(v =>
                            v.Affects.Select(a => new SbomPackageVulnerability()
                            {
                                VulnerabilityId = v.Id,
                                SbomPackageId = bomRefToId[a.Ref],
                            })
                        )
                        .ToList() ?? [];

                var createdDeps = BuildDependencies(edges, bomRefToId);

                _db.SbomPackages.AddRange(packages);
                _db.PackageDependencies.AddRange(createdDeps);
                _db.SbomPackageVulnerabilities.AddRange(packageVulnerabilities);

                projectBranch.PackageCount = packages.Count;
                projectBranch.VulnerabilityCount = packageVulnerabilities
                    .DistinctBy(x => x.SbomPackageId)
                    .Count();
                projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;
                projectBranch.ProcessStep = Shared.Model.Enums.ProcessStep.Processed;

                await _db.SaveChangesAsync(context.CancellationToken);
                _db.ChangeTracker.Clear();

                // Update package severities based on associated vulnerabilities

                var packagesToTransform = _db
                    .SbomPackages.Include(x => x.Vulnerabilities)
                    .Where(x => x.SbomId == sbom.Id);

                foreach (var pkg in packagesToTransform)
                {
                    pkg.Severity =
                        pkg.Vulnerabilities.Select(v => v.Severity)
                            .OrderByDescending(s => SeverityRank.GetValueOrDefault(s, 0))
                            .FirstOrDefault() ?? "None";
                }

                await _db.SaveChangesAsync(context.CancellationToken);

                _logger.LogDebug(
                    "Ingestion finished successfully. Packages: {pkgCount}, Deps: {depCount}",
                    packages.Count,
                    createdDeps.Count
                );
            }
            catch
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed.");
            projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            await _db.SaveChangesAsync(context.CancellationToken);
            throw;
        }
    }

    private async Task IngestVulnerablities(
        CycloneDxBom bom,
        CancellationToken cancellationToken = default
    )
    {
        var bomVulns = bom.Vulnerabilities ?? [];
        if (bomVulns.Count == 0)
            return;

        var incoming = bomVulns
            .Where(x => !string.IsNullOrWhiteSpace(x.Id))
            .GroupBy(x => x.Id!, StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var x = g.First();
                return new Vulnerability
                {
                    Id = x.Id!,
                    Description = x.Description,
                    Recommendation = x.Recommendation,
                    Severity =
                        (x.Ratings ?? [])
                            .GroupBy(r => r.Severity)
                            .OrderByDescending(gr => gr.Count())
                            .Select(gr => gr.Key)
                            .FirstOrDefault() ?? "Unknown",
                };
            })
            .ToList();

        if (incoming.Count == 0)
            return;

        var incomingIds = incoming.Select(v => v.Id).ToList();

        const int maxRetries = 10;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var existingIds = await _db
                .Vulnerabilities.Where(v => incomingIds.Contains(v.Id))
                .Select(v => v.Id)
                .ToListAsync(cancellationToken);

            var toInsert = incoming.Where(v => !existingIds.Contains(v.Id)).ToList();

            if (toInsert.Count == 0)
                return;

            _db.Vulnerabilities.AddRange(toInsert);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateException) when (attempt < maxRetries)
            {
                _db.ChangeTracker.Clear();
                continue;
            }
        }
    }

    private static List<SbomPackage> BuildPackages(Guid sbomId, CycloneDxBom bom)
    {
        var comps = bom.Components ?? [];
        var packages = new List<SbomPackage>(comps.Count + 1);

        var rootRef = bom.Metadata?.Root?.BomRef ?? "project-root";

        packages.Add(
            new SbomPackage
            {
                Id = Guid.NewGuid(),
                SbomId = sbomId,
                Name = "ProjectRoot",
                Version = null,
                Purl = null,
                Ecosystem = "None",
                Type = "ProjectRoot",
                BomRef = rootRef,
            }
        );

        foreach (var x in comps)
        {
            packages.Add(
                new SbomPackage
                {
                    Id = Guid.NewGuid(),
                    SbomId = sbomId,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? "No Name Found" : x.Name,
                    Version = string.IsNullOrWhiteSpace(x.Version) ? null : x.Version,
                    Purl = string.IsNullOrWhiteSpace(x.Purl) ? null : x.Purl,
                    Ecosystem = InferEcosystemFromPurl(x.Purl),
                    Type = x.Type,
                    BomRef = x.BomRef,
                }
            );
        }

        return packages;
    }

    private static Dictionary<string, List<string>> BuildEdges(CycloneDxBom bom)
    {
        var edges = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        if (bom.Dependencies is null)
            return edges;

        foreach (var d in bom.Dependencies)
        {
            if (string.IsNullOrWhiteSpace(d.Ref))
                continue;

            if (!edges.TryGetValue(d.Ref, out var list))
            {
                list = [];
                edges[d.Ref] = list;
            }

            if (d.DependsOn is null)
                continue;

            foreach (var child in d.DependsOn)
            {
                if (!string.IsNullOrWhiteSpace(child))
                    list.Add(child);
            }
        }

        return edges;
    }

    private static HashSet<PackageDependency> BuildDependencies(
        Dictionary<string, List<string>> edges,
        Dictionary<string, Guid> bomRefToId
    )
    {
        var created = new HashSet<PackageDependency>();

        foreach (var (parentRef, children) in edges)
        {
            if (!bomRefToId.TryGetValue(parentRef, out var parentId))
                continue;

            foreach (var childRef in children)
            {
                if (!bomRefToId.TryGetValue(childRef, out var childId))
                    continue;

                created.Add(new PackageDependency { ParentId = parentId, ChildId = childId });
            }
        }

        return created;
    }

    private static string? InferEcosystemFromPurl(string? purl)
    {
        if (string.IsNullOrWhiteSpace(purl))
            return "Unknown";

        const string prefix = "pkg:";
        if (!purl.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return "Unknown";

        var slash = purl.IndexOf('/', prefix.Length);
        if (slash < 0)
            return "Unknown";

        var type = purl[prefix.Length..slash].ToLowerInvariant();

        return type;
    }

    public static readonly Dictionary<string, int> SeverityRank = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["None"] = 0,
        ["low"] = 1,
        ["medium"] = 2,
        ["high"] = 3,
        ["critical"] = 4,
    };
}
