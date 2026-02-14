using DepVis.Core.Dtos;
using DepVis.Core.Repositories;
using DepVis.Shared.Model;

namespace DepVis.Core.Services;

public class GraphService(SbomRepository repo)
{
    public async Task<GraphDataDto?> GetProjectGraphData(
        Guid branchId,
        bool showAllParents = true,
        string? severityFilter = null,
        Guid? commitId = null
    )
    {
        var sbom = await repo.GetLatestWithPackagesAndChildrenAsync(commitId ?? branchId);

        if (sbom == null)
            return null;

        if (!string.IsNullOrEmpty(severityFilter))
        {
            var sbomPackages = sbom.SbomPackages.Where(x => x.Severity == severityFilter);

            if (sbomPackages == null)
                return null;

            HashSet<PackageRelationDto> relationsNew = [];
            HashSet<PackageDto> packagesNew = [];

            foreach (var pkg in sbomPackages)
            {
                var result = GetToRootPath(pkg, sbom, showAllParents, severityFilter);
                if (result == null)
                    continue;
                packagesNew.UnionWith(result.Packages);
                relationsNew.UnionWith(result.Relationships);
            }

            return new GraphDataDto
            {
                Packages = packagesNew.ToList(),
                Relationships = relationsNew.ToList(),
            };
        }

        var relations = sbom
            .SbomPackages.SelectMany(pkg =>
                pkg.Children.Select(child => new PackageRelationDto
                {
                    To = child.ChildId,
                    From = child.ParentId,
                })
            )
            .ToList();

        // Also pick the highest Severity out of all vulnerabilities

        var packages = sbom
            .SbomPackages.Select(x => new PackageDto
            {
                Name = x.Name,
                Id = x.Id,
                Severity = x.Severity,
            })
            .ToList();

        return new GraphDataDto { Packages = packages, Relationships = relations };
    }

    public async Task<GraphDataDto?> GetPackageHierarchyGraphData(
        Guid branchId,
        Guid packageId,
        Guid? commitId = null
    )
    {
        var sbom = await repo.GetLatestWithPackagesAndParentsAsync(commitId ?? branchId);
        if (sbom == null)
            return null;

        return GetToRootPath(sbom.SbomPackages.Where(x => x.Id == packageId).First(), sbom);
    }

    private static GraphDataDto? GetToRootPath(
        SbomPackage destinationPackage,
        Sbom sbom,
        bool showAllParents = true,
        string? severityFilter = null
    )
    {
        var relations = new List<PackageRelationDto>();
        var packages = new List<PackageDto>();
        var processed = new HashSet<Guid>();
        var stack = new Stack<SbomPackage>([destinationPackage]);

        while (stack.TryPop(out var pkg))
        {
            if (processed.Contains(pkg.Id))
                continue;
            processed.Add(pkg.Id);

            var newPackage = new PackageDto
            {
                Name = pkg.Name,
                Id = pkg.Id,
                Severity = pkg.Severity,
            };

            if (!string.IsNullOrEmpty(severityFilter))
            {
                newPackage.Severity =
                    newPackage.Severity == severityFilter ? severityFilter : "None";
            }

            packages.Add(newPackage);

            var parents = sbom
                .SbomPackages.Where(x => pkg.Parents.Select(p => p.Parent.Id).Contains(x.Id))
                .ToList();

            if (!showAllParents)
            {
                var parent = parents.FirstOrDefault();
                if (parent != null)
                {
                    relations.Add(new PackageRelationDto { To = parent.Id, From = pkg.Id });
                    stack.Push(parent);
                }
                continue;
            }

            foreach (var parent in parents)
            {
                relations.Add(new PackageRelationDto { To = parent.Id, From = pkg.Id });
                if (!processed.Contains(parent.Id))
                    stack.Push(parent);
            }
        }

        return new GraphDataDto { Packages = packages, Relationships = relations };
    }
}
