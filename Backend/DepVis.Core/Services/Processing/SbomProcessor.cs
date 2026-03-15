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
        bool skipGraphBuilding = false,
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
            packageBuild.RootBomRef,
            skipGraphBuilding
        );

        var vulnerabilityBuild = packageVulnerabilityMapper.Map(
            bom,
            packageBuild.Packages,
            graphBuild.BomRefToId
        );

        db.SbomPackages.AddRange(packageBuild.Packages);

        if (!skipGraphBuilding)
        {
            db.PackageDependencies.AddRange(graphBuild.Dependencies);
        }

        var distinctVulnerabilities = vulnerabilityBuild.PackageVulnerabilities.Distinct().ToList();

        db.SbomPackageVulnerabilities.AddRange(distinctVulnerabilities);

        await db.SaveChangesAsync(cancellationToken);

        return new SbomProcessingResult
        {
            Packages = packageBuild.Packages,
            Dependencies = graphBuild.Dependencies,
            PackageVulnerabilities = distinctVulnerabilities,
            EcoSystems =
            [
                .. packageBuild
                    .Packages.Where(x => !string.IsNullOrEmpty(x.Ecosystem))
                    .GroupBy(x => x.Ecosystem)
                    .OrderByDescending(g => g.Count())
                    .Select(p => p.Key!),
            ],
        };
    }
}
