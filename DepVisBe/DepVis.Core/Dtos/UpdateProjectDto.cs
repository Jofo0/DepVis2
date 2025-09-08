using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Dtos;

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; } = ProjectType.Folder;
    public ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Created;
    public string ProjectLink { get; set; } = string.Empty;
}