namespace DepVis.Shared.Model;

public class PackageDependency
{
    public Guid ParentId { get; set; }
    public SbomPackage Parent { get; set; } = null!;

    public Guid ChildId { get; set; }
    public SbomPackage Child { get; set; } = null!;
}
