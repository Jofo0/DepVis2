using DepVis.Shared.Model;

namespace DepVis.Core.Services;

public interface ISbomPackageBuilder
{
    PackageBuildResult Build(Guid sbomId, CycloneDxBom bom);
}

public record PackageBuildResult(
    List<SbomPackage> Packages,
    List<PackagesDuplicatesResolve> DuplicateResolutions,
    string RootBomRef
);

public record PackagesDuplicatesResolve(List<string> BomRefs, Guid Id);
