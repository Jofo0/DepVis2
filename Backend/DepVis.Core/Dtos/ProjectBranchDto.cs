namespace DepVis.Core.Dtos;

public class ProjectBranchDto
{
    public List<BranchItemDto> Items { get; set; } = [];
    public int Initiated = 0;
    public int SbomGenerated = 0;
    public int SbomIngested = 0;
    public int Complete = 0;
    public int TotalCount = 0;
}

public class BranchItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
    public bool IsTag { get; set; } = false;
}
