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

            await using var tx = await _db.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable,
                context.CancellationToken
            );

            try
            {
                var vulnerabilities = BuildVulnerabilities(bom);

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

                _db.Vulnerabilities.AddRange(vulnerabilities);
                _db.SbomPackages.AddRange(packages);
                _db.PackageDependencies.AddRange(createdDeps);
                _db.SbomPackageVulnerabilities.AddRange(packageVulnerabilities);

                projectBranch.PackageCount = packages.Count;
                projectBranch.VulnerabilityCount = packageVulnerabilities
                    .DistinctBy(x => x.SbomPackageId)
                    .Count();
                projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;

                await _db.SaveChangesAsync(context.CancellationToken);
                await tx.CommitAsync(context.CancellationToken);
                _logger.LogDebug(
                    "Ingestion finished successfully. Packages: {pkgCount}, Deps: {depCount}",
                    packages.Count,
                    createdDeps.Count
                );
            }
            catch
            {
                await tx.RollbackAsync(context.CancellationToken);
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

    private List<Vulnerability> BuildVulnerabilities(CycloneDxBom bom)
    {
        var vulnerabilities =
            bom.Vulnerabilities?.Select(x => new Vulnerability
                {
                    Id = x.Id,
                    Description = x.Description,
                    Recommendation = x.Recommendation,
                    Severity =
                        x.Ratings.GroupBy(r => r.Severity)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .FirstOrDefault() ?? "Unknown",
                })
                .ToList() ?? [];

        var existingIds = _db.Vulnerabilities.Select(v => v.Id).ToHashSet();

        return [.. vulnerabilities.Where(v => !existingIds.Contains(v.Id))];
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
        var created = new HashSet<PackageDependency>(); // relies on PackageDependency equality (ParentId, ChildId)

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
}
