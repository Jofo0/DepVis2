using System.Text.Json;
using CommunityToolkit.HighPerformance;
using DepVis.Core.Context;
using DepVis.Core.Util;
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

        if (context.Message.IsHistory)
        {
            await ProcessForHistory(context.Message.SbomId, context.CancellationToken);
        }
        else
        {
            await ProcessForBranch(context.Message.SbomId, context.CancellationToken);
        }
        _logger.LogDebug("Ingestion completed.");
    }

    public async Task ProcessForBranch(Guid sbomId, CancellationToken cancellationToken = default)
    {
        var sbom = await _db
            .Sboms.Include(x => x.ProjectBranch)
            .FirstAsync(x => x.Id == sbomId, cancellationToken);

        var projectBranch = sbom.ProjectBranch;

        if (projectBranch == null)
            return;

        try
        {
            projectBranch.ProcessStep = Shared.Model.Enums.ProcessStep.SbomIngest;
            projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
            await _db.SaveChangesAsync(cancellationToken);

            var result = await ProcessSbom(sbom.FileName, sbom.Id, cancellationToken);

            projectBranch.PackageCount = result.Packages.Count;
            projectBranch.VulnerabilityCount = result
                .PackageVulnerabilities.DistinctBy(x => x.SbomPackageId)
                .Count();

            projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;
            projectBranch.ProcessStep = Shared.Model.Enums.ProcessStep.Processed;
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed.");
            projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            await _db.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public async Task ProcessForHistory(Guid sbomId, CancellationToken cancellationToken = default)
    {
        var sbom = await _db
            .Sboms.Include(x => x.BranchHistory)
            .Include(x => x.ProjectBranch)
            .FirstAsync(x => x.Id == sbomId, cancellationToken);

        var branchHistory = sbom.BranchHistory;
        var projectBranch = sbom.ProjectBranch;

        if (projectBranch == null || branchHistory == null)
            return;
        try
        {
            projectBranch.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.SbomIngest;
            projectBranch.HistoryProcessinStatus = Shared.Model.Enums.ProcessStatus.Pending;
            branchHistory.ProcessStatus = Shared.Model.Enums.ProcessStatus.Pending;
            await _db.SaveChangesAsync(cancellationToken);

            var result = await ProcessSbom(sbom.FileName, sbom.Id, cancellationToken);

            branchHistory.PackageCount = result.Packages.Count;
            branchHistory.VulnerabilityCount = result
                .PackageVulnerabilities.DistinctBy(x => x.SbomPackageId)
                .Count();

            projectBranch.HistoryProcessinStatus = Shared.Model.Enums.ProcessStatus.Success;
            projectBranch.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.Processed;
            branchHistory.ProcessStatus = Shared.Model.Enums.ProcessStatus.Success;

            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion failed.");
            projectBranch.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            branchHistory.ProcessStatus = Shared.Model.Enums.ProcessStatus.Failed;
            await _db.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public class ProcessingResult
    {
        public List<SbomPackage> Packages { get; set; } = [];
        public HashSet<PackageDependency> Dependencies { get; set; } = [];
        public List<SbomPackageVulnerability> PackageVulnerabilities { get; set; } = [];
    }

    private async Task<ProcessingResult> ProcessSbom(
        string sbomFile,
        Guid sbomId,
        CancellationToken cancellationToken = default
    )
    {
        await using var cycloneDxStream = await _minio.RetrieveAsync(sbomFile, cancellationToken);

        CycloneDxBom bom =
            JsonSerializer.Deserialize<CycloneDxBom>(cycloneDxStream, _jsonOptions)
            ?? new CycloneDxBom { Components = [] };

        await IngestVulnerablities(bom);

        var packages = BuildPackages(sbomId, bom);

        var edges = BuildEdges(bom);
        var bomRefToId = packages.ToDictionary(p => p.BomRef, p => p.Id, StringComparer.Ordinal);

        var maxRankByBomRef = new Dictionary<string, int>(StringComparer.Ordinal);

        if (bom.Vulnerabilities is not null)
        {
            foreach (var v in bom.Vulnerabilities)
            {
                var rank = SeveritySort.SeverityRank.GetValueOrDefault(
                    (v.Ratings ?? [])
                        .GroupBy(r => r.Severity)
                        .OrderByDescending(gr => gr.Count())
                        .Select(gr => gr.Key)
                        .FirstOrDefault() ?? "Unknown",
                    0
                );

                foreach (var a in v.Affects ?? [])
                {
                    if (string.IsNullOrWhiteSpace(a.Ref))
                        continue;

                    if (!maxRankByBomRef.TryGetValue(a.Ref, out var existing) || rank > existing)
                        maxRankByBomRef[a.Ref] = rank;
                }
            }
        }

        foreach (var p in packages)
        {
            if (p.BomRef is null)
            {
                p.Severity = "None";
                continue;
            }

            var rank = maxRankByBomRef.GetValueOrDefault(p.BomRef, 0);

            p.Severity = rank switch
            {
                4 => "critical",
                3 => "high",
                2 => "medium",
                1 => "low",
                _ => "None",
            };
        }

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
        await _db.SaveChangesAsync(cancellationToken);

        return new()
        {
            Packages = packages,
            Dependencies = createdDeps,
            PackageVulnerabilities = packageVulnerabilities,
        };
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
                return new Shared.Model.Vulnerability
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
                var addedEntries = _db
                    .ChangeTracker.Entries<Vulnerability>()
                    .Where(e => e.State == EntityState.Added)
                    .ToList();

                foreach (var entry in addedEntries)
                {
                    entry.State = EntityState.Detached;
                }
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
}
