using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories;
using DepVis.Core.Util;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Services;

public class PackageService(PackageRepository repo)
{
    public async Task<PackagesDto> GetPackageData(
        Guid id,
        ODataQueryOptions<SbomPackage> odata,
        Guid? commitId = null
    )
    {
        IQueryable<SbomPackage> packages = odata.ApplyOdataIEnumerable(
            repo.GetLatestPackagesForBranchOrCommit(commitId ?? id)
        );

        var retrieved = await packages.ToListAsync();

        var ecosystemGroups = retrieved
            .GroupBy(x => x.Ecosystem)
            .Select(g => new NameCount { Name = g.Key ?? "", Count = g.Count() })
            .OrderBy(x => x.Count)
            .ToList();

        var vulnerableCounts = retrieved
            .GroupBy(x => x.Vulnerabilities.Count > 0)
            .Select(g => new NameCount
            {
                Name = g.Key ? Constants.VulnerablePackage : Constants.NotVulnerablePackage,
                Count = g.Count(),
            })
            .OrderBy(x => x.Count)
            .ToList();

        var depthCounts = retrieved
            .GroupBy(x =>
                x.Depth == 1 || x.Depth == 0 ? Constants.Manifests
                : x.Depth == 2 ? Constants.Direct
                : Constants.Transitive
            )
            .Select(g => new NameCount { Name = g.Key, Count = g.Count() })
            .OrderBy(x => x.Count)
            .ToList();

        return new()
        {
            Vulnerabilities = vulnerableCounts,
            EcoSystems = ecosystemGroups,
            Depths = depthCounts,
            PackageItems = [.. retrieved.Select(x => x.MapToPackageItemDto())],
        };
    }

    public async Task<PackageDetailedDto?> GetPackageData(
        Guid packageId,
        CancellationToken cancellation
    )
    {
        var package = await repo.GetPackage(packageId, cancellation);

        if (package == null)
        {
            return null;
        }

        return new()
        {
            Id = package.Id,
            Ecosystem = package.Ecosystem ?? "",
            Name = package.Name,
            Version = package.Version ?? "",
            Vulnerabilities =
            [
                .. package
                    .Vulnerabilities.Select(vuln => new VulnerabilityDetailedDto()
                    {
                        Id = vuln.Id,
                        Description = vuln.Description,
                        Recommendation = vuln.Recommendation,
                        Severity = vuln.Severity,
                    })
                    .OrderByDescending(x =>
                        SeveritySort.SeverityRank.GetValueOrDefault(x.Severity, 0)
                    ),
            ],
        };
    }
}
