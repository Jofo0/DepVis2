using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; } = ProjectType.Folder;
    public ProcessStep ProcessStatus { get; set; } = ProcessStep.Created;
    public string ProjectLink { get; set; } = string.Empty;
}
