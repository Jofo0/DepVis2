namespace DepVis.Shared.Model;

public class ProjectStatistics
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
}
