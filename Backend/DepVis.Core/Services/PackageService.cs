using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Services;

public class PackageService(PackageRepository repo)
{
    public async Task<PackagesDto> GetPackageData(Guid id, ODataQueryOptions<SbomPackage> odata)
    {
        var packages = odata.ApplyOdataIEnumerable(repo.GetLatestPackagesForBranch(id));

        var ecosystemGroups = await packages
            .GroupBy(x => x.Ecosystem)
            .Select(g => new NameCount { Name = g.Key ?? "", Count = g.Count() })
            .OrderBy(x => x.Count)
            .ToListAsync();

        var vulnerableCounts = await packages
            .GroupBy(x => x.Vulnerabilities.Count > 0)
            .Select(g => new NameCount { Name = g.Key ? "Vulnerable" : "OK", Count = g.Count() })
            .OrderBy(x => x.Count)
            .ToListAsync();

        var retrieved = await packages.ToListAsync();

        return new()
        {
            Vulnerabilities = vulnerableCounts,
            EcoSystems = ecosystemGroups,
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
                        x.Severity switch
                        {
                            "Critical" => 4,
                            "High" => 3,
                            "Medium" => 2,
                            "Low" => 1,
                            _ => 0,
                        }
                    ),
            ],
        };
    }
}
