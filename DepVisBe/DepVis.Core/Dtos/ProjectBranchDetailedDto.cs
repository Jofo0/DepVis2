namespace DepVis.Core.Dtos;

public class ProjectBranchDetailedDto
{
    public Guid Id { get; set; }
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string CommitMessage { get; set; } = string.Empty;
    public DateTime CommitDate { get; set; } = DateTime.Now;
    public DateTime ScanDate { get; set; } = DateTime.Now;
    public string CommitSha { get; set; } = string.Empty;
}
