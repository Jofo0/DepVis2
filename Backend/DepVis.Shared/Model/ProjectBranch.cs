using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Model;

public class ProjectBranch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProcessStep ProcessStep { get; set; } = ProcessStep.Created;
    public ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Success;
    public bool IsTag { get; set; } = false;
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
    public ProcessStep HistoryProcessingStep { get; set; } = ProcessStep.NotStarted;
    public ProcessStatus HistoryProcessinStatus { get; set; } = ProcessStatus.Success;
    public DateTime CommitDate { get; set; }
    public DateTime ScanDate { get; set; }
    public string CommitMessage { get; set; } = string.Empty;
    public string CommitSha { get; set; } = string.Empty;
    public Project Project { get; set; } = null!;
    public ICollection<Sbom> Sboms { get; set; } = [];
}
