namespace DepVis.Core.Dtos;

public class ProjectBranchDetailedDto
{
    public Guid Id { get; set; }
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
}
