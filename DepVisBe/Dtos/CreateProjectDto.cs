using DepVisBe.Model.Enums;

namespace DepVisBe.Dtos;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; } = ProjectType.GitHub;
    public string FolderPath { get; set; } = string.Empty;
    public string GitHubLink { get; set; } = string.Empty;
}