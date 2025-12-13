using DepVis.Core.Dtos;
using DepVis.Core.Repositories;
using DepVis.Shared.Model;

namespace DepVis.Core.Services;

public class GraphService(SbomRepository repo)
{
    public async Task<GraphDataDto?> GetProjectGraphData(Guid branchId)
    {
        var sbom = await repo.GetLatestWithPackagesAndChildrenAsync(branchId);
        if (sbom == null)
            return null;

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
                Severity = x.Vulnerabilities.FirstOrDefault()?.Severity ?? "None",
            })
            .ToList();

        return new GraphDataDto { Packages = packages, Relationships = relations };
    }

    public async Task<GraphDataDto?> GetPackageHierarchyGraphData(Guid branchId, Guid packageId)
    {
        var sbom = await repo.GetLatestWithPackagesAndParentsAsync(branchId);
        if (sbom == null)
            return null;

        var relations = new List<PackageRelationDto>();
        var packages = new List<PackageDto>();
        var processed = new HashSet<Guid>();
        var stack = new Stack<SbomPackage>(sbom.SbomPackages.Where(x => x.Id == packageId));

        while (stack.TryPop(out var pkg))
        {
            if (processed.Contains(pkg.Id))
                continue;
            processed.Add(pkg.Id);

            packages.Add(new PackageDto { Name = pkg.Name, Id = pkg.Id });

            var parents = sbom
                .SbomPackages.Where(x => pkg.Parents.Select(p => p.Parent.Id).Contains(x.Id))
                .ToList();

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
