using DepVis.Shared.Model.Enums;

namespace DepVis.Shared.Model;

public class BranchHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectBranchId { get; set; }
    public int PackageCount { get; set; } = 0;
    public int VulnerabilityCount { get; set; } = 0;
    public DateTime CommitDate { get; set; }
    public string CommitMessage { get; set; } = string.Empty;
    public string CommitSha { get; set; } = string.Empty;
    public ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Pending;
    public ProjectBranch ProjectBranch { get; set; } = null!;
    public ICollection<Sbom> Sboms { get; set; } = [];
}
