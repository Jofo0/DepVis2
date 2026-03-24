using DepVis.Shared.Model;

namespace DepVis.Core.Services.Interfaces;

public interface ISbomProcessor
{
    Task<SbomProcessingResult> ProcessAsync(
        Sbom sbom,
        bool skipGraphBuilding = false,
        CancellationToken cancellationToken = default
    );
}

public class SbomProcessingResult
{
    public List<SbomPackage> Packages { get; set; } = [];
    public HashSet<PackageDependency> Dependencies { get; set; } = [];
    public List<SbomPackageVulnerability> PackageVulnerabilities { get; set; } = [];
    public List<SbomPackageVulnerability> DirectVulnerabilities { get; set; } = [];
    public List<string> EcoSystems { get; set; } = [];
}
