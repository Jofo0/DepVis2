namespace DepVis.Shared.Model;

public class PackageDependency : IEquatable<PackageDependency>
{
    public Guid ParentId { get; set; }
    public SbomPackage Parent { get; set; } = null!;

    public Guid ChildId { get; set; }
    public SbomPackage Child { get; set; } = null!;

    public bool Equals(PackageDependency? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return ParentId.Equals(other.ParentId) && ChildId.Equals(other.ChildId);
    }

    public override bool Equals(object? obj) => Equals(obj as PackageDependency);

    public override int GetHashCode() => HashCode.Combine(ParentId, ChildId);

    public static bool operator ==(PackageDependency? left, PackageDependency? right) =>
        Equals(left, right);

    public static bool operator !=(PackageDependency? left, PackageDependency? right) =>
        !Equals(left, right);
}
