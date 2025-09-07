using DepVis.Core.Model.Enums;

namespace DepVis.Core.Dtos;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; } = ProjectType.GitHub;
    public string ProjectLink { get; set; } = string.Empty;
}