using DepVis.Shared.Model;

namespace DepVis.Core.Services;

public interface IDependencyGraphBuilder
{
    DependencyGraphBuildResult Build(
        CycloneDxBom bom,
        IReadOnlyList<SbomPackage> packages,
        IReadOnlyList<PackagesDuplicatesResolve> duplicateResolutions,
        string rootBomRef
    );
}

public record DependencyGraphBuildResult(
    Dictionary<string, Guid> BomRefToId,
    HashSet<PackageDependency> Dependencies
);
