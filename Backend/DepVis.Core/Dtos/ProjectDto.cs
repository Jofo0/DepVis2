using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; }
    public ProcessStatus ProcessStatus { get; set; }
    public ProcessStep ProcessStep { get; set; }
    public string ProjectLink { get; set; } = string.Empty;
}
