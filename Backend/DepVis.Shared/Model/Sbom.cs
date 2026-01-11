namespace DepVis.Shared.Model;

public class Sbom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CommitDate { get; set; } = DateTime.UtcNow;
    public string CommitMessage { get; set; } = string.Empty;
    public string CommitSha { get; set; } = string.Empty;
    public Guid? ProjectBranchId { get; set; }
    public Guid? BranchHistoryId { get; set; }
    public ProjectBranch ProjectBranch { get; set; } = null!;
    public BranchHistory? BranchHistory { get; set; } = null;
    public ICollection<SbomPackage> SbomPackages { get; set; } = [];
}
