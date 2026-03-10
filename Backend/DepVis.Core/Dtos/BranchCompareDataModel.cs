namespace DepVis.Core.Dtos;

public record BranchCompareDataModel(
    Guid Id,
    List<SmallPackage> PackageNames,
    List<string> VulnerabilityIds
);

public record SmallPackage(string Name, string Version, string Ecosystem)
{
    public virtual bool Equals(SmallPackage? package)
    {
        return package is not null && Name == package.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}

public record BranchCompareDto(
    List<string> AddedPackages,
    List<string> RemovedPackages,
    List<NameCount> AddedEcosystemse,
    List<NameCount> RemovedEcosystems,
    List<string> AddedVulnerabilityIds,
    List<string> RemovedVulnerabilityIds,
    int mainBranchPackageCount,
    int comparedBranchPackageCount,
    int mainBranchVulnerabilityCount,
    int comparedBranchVulnerabilityCount
);
