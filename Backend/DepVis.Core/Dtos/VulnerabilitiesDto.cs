namespace DepVis.Core.Dtos;

public class VulnerabilitiesDto
{
    public List<VulnerabilitySmallDto> Vulnerabilities { get; set; } = new();
    public List<NameCount> Risks { get; set; } = new();
}
