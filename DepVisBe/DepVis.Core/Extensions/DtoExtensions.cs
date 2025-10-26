using DepVis.Core.Dtos;
using DepVis.Shared.Model;

namespace DepVis.Core.Extensions;

public static class DtoExtensions
{
    public static ProjectDto MapToDto(this Project project) =>
        new()
        {
            Id = project.Id,
            Name = project.Name,
            ProjectType = project.ProjectType,
            ProjectLink = project.ProjectLink,
        };

    public static ProjectStatsDto MapToDto(this ProjectBranches stats) =>
        new() { PackageCount = stats.PackageCount, VulnerabilityCount = stats.VulnerabilityCount };

    public static ProjectBranchDto MapToBranchesDto(this ProjectBranches pb) =>
        new() { Id = pb.Id, Name = pb.Name };

    public static PackageDetailedDto MapToPackagesDetailed(this SbomPackage pb) =>
        new()
        {
            Id = pb.Id,
            Name = pb.Name,
            Ecosystem = pb.Ecosystem,
            Version = pb.Version,
            Vulnerable = pb.Vulnerabilities.Count > 0,
        };

    public static ProjectBranchDetailedDto MapToBranchesDetailedDto(this ProjectBranches pb)
    {
        var latestSbom = pb.Sboms.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        // TODO: set this in db to be on the Project
        return new()
        {
            Id = pb.Id,
            Name = pb.Name,
            PackageCount = pb.PackageCount,
            VulnerabilityCount = pb.VulnerabilityCount,
            CommitDate = latestSbom.CommitDate,
            CommitMessage = latestSbom.CommitMessage,
            CommitSha = latestSbom.CommitSha,
            ScanDate = latestSbom.CreatedAt,
        };
    }
}
