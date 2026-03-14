using DepVis.Shared.Model;

namespace DepVis.Core.Services.Processing;

public class DependencyGraphBuilder : IDependencyGraphBuilder
{
    public DependencyGraphBuildResult Build(
        CycloneDxBom bom,
        IReadOnlyList<SbomPackage> packages,
        IReadOnlyList<PackagesDuplicatesResolve> duplicateResolutions,
        string rootBomRef,
        bool skipDependencies = false
    )
    {
        var edges = BuildEdges(bom);
        var depths = CalculateDepths(edges, rootBomRef);

        ApplyDepths(packages, duplicateResolutions, depths);

        return new DependencyGraphBuildResult(
            BuildBomRefToId(packages, duplicateResolutions),
            skipDependencies
                ? []
                : BuildDependencies(edges, BuildBomRefToId(packages, duplicateResolutions))
        );
    }

    private static Dictionary<string, List<string>> BuildEdges(CycloneDxBom bom)
    {
        var edges = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        if (bom.Dependencies is null)
            return edges;

        foreach (var dependency in bom.Dependencies)
        {
            if (string.IsNullOrWhiteSpace(dependency.Ref))
                continue;

            if (!edges.TryGetValue(dependency.Ref, out var children))
            {
                children = [];
                edges[dependency.Ref] = children;
            }

            if (dependency.DependsOn is null)
                continue;

            foreach (var child in dependency.DependsOn)
            {
                if (!string.IsNullOrWhiteSpace(child))
                    children.Add(child);
            }
        }

        return edges;
    }

    private static Dictionary<string, int> CalculateDepths(
        Dictionary<string, List<string>> edges,
        string root
    )
    {
        var depths = new Dictionary<string, int>(StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();

        queue.Enqueue(root);
        visited.Add(root);
        depths[root] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentDepth = depths[current];

            if (!edges.TryGetValue(current, out var neighbors))
                continue;

            foreach (var child in neighbors)
            {
                if (!visited.Add(child))
                    continue;

                depths[child] = currentDepth + 1;
                queue.Enqueue(child);
            }
        }

        return depths;
    }

    private static void ApplyDepths(
        IReadOnlyList<SbomPackage> packages,
        IReadOnlyList<PackagesDuplicatesResolve> duplicateResolutions,
        IReadOnlyDictionary<string, int> depths
    )
    {
        var packagesByBomRef = packages
            .Where(p => p.BomRef is not null)
            .ToDictionary(p => p.BomRef!, StringComparer.Ordinal);

        var packagesById = packages.ToDictionary(p => p.Id);

        var duplicateResolutionByBomRef = duplicateResolutions
            .SelectMany(x => x.BomRefs.Select(bomRef => new { bomRef, x.Id }))
            .ToDictionary(x => x.bomRef, x => x.Id, StringComparer.Ordinal);

        foreach (var (bomRef, depth) in depths)
        {
            if (packagesByBomRef.TryGetValue(bomRef, out var package))
            {
                SetMinDepth(package, depth);
                continue;
            }

            if (
                duplicateResolutionByBomRef.TryGetValue(bomRef, out var packageId)
                && packagesById.TryGetValue(packageId, out var resolvedPackage)
            )
            {
                SetMinDepth(resolvedPackage, depth);
            }
        }
    }

    private static Dictionary<string, Guid> BuildBomRefToId(
        IReadOnlyList<SbomPackage> packages,
        IReadOnlyList<PackagesDuplicatesResolve> duplicateResolutions
    )
    {
        var bomRefToId = packages
            .Where(p => !string.IsNullOrWhiteSpace(p.BomRef))
            .ToDictionary(p => p.BomRef!, p => p.Id, StringComparer.Ordinal);

        foreach (var resolution in duplicateResolutions)
        {
            foreach (var bomRef in resolution.BomRefs)
            {
                bomRefToId[bomRef] = resolution.Id;
            }
        }

        return bomRefToId;
    }

    private static HashSet<PackageDependency> BuildDependencies(
        Dictionary<string, List<string>> edges,
        IReadOnlyDictionary<string, Guid> bomRefToId
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

    private static void SetMinDepth(SbomPackage package, int depth)
    {
        if (package.Depth is null || package.Depth > depth)
            package.Depth = depth;
    }
}
