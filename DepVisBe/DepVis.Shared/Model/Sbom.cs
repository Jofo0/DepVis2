namespace DepVis.Shared.Model;

public class Sbom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Branch { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<SbomPackage> SbomPackages { get; set; } = [];
}
