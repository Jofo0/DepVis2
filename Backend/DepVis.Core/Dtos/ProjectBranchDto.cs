namespace DepVis.Core.Dtos;

public class ProjectBranchDto
{
    public List<BranchItemDto> Items { get; set; } = [];
    public int Initiated { get; set; } = 0;
    public int SbomGenerated { get; set; } = 0;
    public int SbomIngested { get; set; } = 0;
    public int Complete { get; set; } = 0;
    public int TotalCount { get; set; } = 0;
}

public class BranchItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
    public bool IsTag { get; set; } = false;
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
    public List<BranchCommitDto> Commits { get; set; } = [];
}
