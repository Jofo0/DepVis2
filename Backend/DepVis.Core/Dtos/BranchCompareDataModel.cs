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
        return package is not null && Name == package.Name && Version == package.Version;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Version);
    }
}

public record PackageInfoDto(string Name, string Version, bool Update);

public record BranchCompareDto(
    List<PackageInfoDto> AddedPackages,
    List<PackageInfoDto> RemovedPackages,
    List<NameCount> AddedEcosystems,
    List<NameCount> RemovedEcosystems,
    List<string> AddedVulnerabilityIds,
    List<string> RemovedVulnerabilityIds,
    int mainBranchPackageCount,
    int comparedBranchPackageCount,
    int mainBranchVulnerabilityCount,
    int comparedBranchVulnerabilityCount
);