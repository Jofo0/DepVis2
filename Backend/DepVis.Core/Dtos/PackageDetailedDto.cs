namespace DepVis.Core.Dtos;

public class PackageDetailedDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Ecosystem { get; set; } = string.Empty;
    public List<VulnerabilityDetailedDto> Vulnerabilities { get; set; } = [];
}
