using DepVisBe.Model.Enums;

namespace DepVisBe.Dtos;

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; } = ProjectType.GitHub;
    public ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Created;
    public string FolderPath { get; set; } = string.Empty;
    public string GitHubLink { get; set; } = string.Empty;
}