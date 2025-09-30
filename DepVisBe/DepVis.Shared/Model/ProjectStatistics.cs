namespace DepVis.Shared.Model;

public class ProjectStatistics
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Branch { get; set; } = string.Empty;
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
    public Guid ProjectId { get; set; }
}
