namespace DepVis.Core.Dtos;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
    public string ProjectLink { get; set; } = string.Empty;
}
