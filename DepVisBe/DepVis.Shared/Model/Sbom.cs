namespace DepVis.Shared.Model;

public class Sbom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid ProjectBranchId { get; set; }
    public ProjectBranches ProjectBranch { get; set; } = null!;
    public ICollection<SbomPackage> SbomPackages { get; set; } = [];
}
