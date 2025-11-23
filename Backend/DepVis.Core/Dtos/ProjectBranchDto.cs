namespace DepVis.Core.Dtos;

public class ProjectBranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
}
