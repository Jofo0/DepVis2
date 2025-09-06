namespace DepVisBe.Dtos;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;
    public string GitHubLink { get; set; } = string.Empty;
}
