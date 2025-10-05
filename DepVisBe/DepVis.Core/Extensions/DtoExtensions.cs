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
}
