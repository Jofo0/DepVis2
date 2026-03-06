using DepVis.Core.Context;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Model;

namespace DepVis.Core.Services.Processing;

public class SbomProcessor(
    ICycloneDxBomLoader bomLoader,
    IVulnerabilityIngestionService vulnerabilityIngestionService,
    ISbomPackageBuilder packageBuilder,
    IDependencyGraphBuilder dependencyGraphBuilder,
    IPackageVulnerabilityMapper packageVulnerabilityMapper,
    DepVisDbContext db
) : ISbomProcessor
{
    public async Task<SbomProcessingResult> ProcessAsync(
        Sbom sbom,
        CancellationToken cancellationToken = default
    )
    {
        var bom = await bomLoader.LoadAsync(sbom.FileName, cancellationToken);

        await vulnerabilityIngestionService.IngestAsync(bom, cancellationToken);

        var packageBuild = packageBuilder.Build(sbom.Id, bom);

        var graphBuild = dependencyGraphBuilder.Build(
            bom,
            packageBuild.Packages,
            packageBuild.DuplicateResolutions,
            packageBuild.RootBomRef
        );

        var vulnerabilityBuild = packageVulnerabilityMapper.Map(
            bom,
            packageBuild.Packages,
            graphBuild.BomRefToId
        );

        db.SbomPackages.AddRange(packageBuild.Packages);
        db.PackageDependencies.AddRange(graphBuild.Dependencies);
        db.SbomPackageVulnerabilities.AddRange(
            vulnerabilityBuild.PackageVulnerabilities.Distinct()
        );

        await db.SaveChangesAsync(cancellationToken);

        return new SbomProcessingResult
        {
            Packages = packageBuild.Packages,
            Dependencies = graphBuild.Dependencies,
            PackageVulnerabilities = vulnerabilityBuild.PackageVulnerabilities,
        };
    }
}
