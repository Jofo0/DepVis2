using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Model;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ProjectType ProjectType { get; set; } = ProjectType.GitHub;
    public ProcessStep ProcessStep { get; set; } = ProcessStep.Created;
    public ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Success;
    public string ProjectLink { get; set; } = string.Empty;
    public ICollection<Sbom> Sboms { get; set; } = [];
    public ICollection<ProjectStatistics> ProjectStatistics { get; set; } = [];
}
