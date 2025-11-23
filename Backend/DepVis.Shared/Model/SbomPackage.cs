namespace DepVis.Shared.Model;

public class SbomPackage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SbomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? Purl { get; set; }
    public string? Ecosystem { get; set; }
    public string? Type { get; set; }
    public string BomRef { get; set; } = string.Empty;
    public ICollection<PackageDependency> Parents { get; set; } = [];
    public ICollection<PackageDependency> Children { get; set; } = [];

    public Sbom Sbom { get; set; } = null!;
    public ICollection<Vulnerability> Vulnerabilities { get; set; } = [];
    public ICollection<SbomPackageVulnerability> SbomPackageVulnerabilities { get; set; } = [];
}
