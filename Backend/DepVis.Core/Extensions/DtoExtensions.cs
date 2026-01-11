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

    public static ProjectStatsDto MapToDto(this ProjectBranch stats) =>
        new() { PackageCount = stats.PackageCount, VulnerabilityCount = stats.VulnerabilityCount };

    public static ProjectBranchDto MapToBranchesDto(this ProjectBranch pb) =>
        new()
        {
            Id = pb.Id,
            Name = pb.Name,
            ProcessStatus = pb.ProcessStatus.ToString(),
            ProcessStep = pb.ProcessStep.ToString(),
            IsTag = pb.IsTag,
        };

    public static PackageItemDto MapToPackageItemDto(this SbomPackage pb) =>
        new()
        {
            Id = pb.Id,
            Name = pb.Name,
            Ecosystem = pb.Ecosystem,
            Version = pb.Version,
            Vulnerable = pb.Vulnerabilities.Count > 0,
        };

    public static ProjectBranchDetailedDto MapToBranchesDetailedDto(this ProjectBranch pb)
    {
        return new()
        {
            Id = pb.Id,
            Name = pb.Name,
            PackageCount = pb.PackageCount,
            VulnerabilityCount = pb.VulnerabilityCount,
            CommitDate = pb.CommitDate,
            CommitMessage = pb.CommitMessage,
            CommitSha = pb.CommitSha,
            ScanDate = pb.ScanDate,
        };
    }

    public static BranchHistoryDto MapToBranchHistoryDto(this ProjectBranch bh)
    {
        var pending = bh.BranchHistories.Any(x =>
            x.ProcessStatus == Shared.Model.Enums.ProcessStatus.Pending
        );

        var processingStep = Shared.Model.Enums.ProcessStep.NotStarted;

        if (pending)
        {
            processingStep = Shared.Model.Enums.ProcessStep.SbomIngest;
        }
        else
        {
            processingStep = bh.HistoryProcessingStep;
        }

        return new()
        {
            Histories =
            [
                .. bh
                    .BranchHistories.OrderBy(x => x.CommitDate)
                    .Select(x => new BranchHistoryEntryDto()
                    {
                        CommitDate = x.CommitDate,
                        CommitMessage = x.CommitMessage,
                        CommitSha = x.CommitSha,
                        PackageCount = x.PackageCount,
                        VulnerabilityCount = x.PackageCount,
                    }),
            ],
            ProcessingStep = processingStep,
        };
    }
}
