using DepVis.Core.Dtos;
using DepVis.Shared.Model;
using DepVis.Shared.Model.Enums;

namespace DepVis.Core.Extensions;

public static class DtoExtensions
{
    public static ProjectDto MapToDto(this Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            ProjectLink = project.ProjectLink,
            EcoSystems =
            [
                .. project.ProjectStatistics?.EcoSystems.Split(",").Where(x => x != "None") ?? []
            ]
        };
    }

    public static ProjectStatsDto MapToDto(this ProjectBranch stats)
    {
        return new ProjectStatsDto { PackageCount = stats.PackageCount, VulnerabilityCount = stats.VulnerabilityCount };
    }

    public static ProjectBranchDto MapToBranchesDto(this List<ProjectBranch> pb)
    {
        var initiatedCount = pb.Where(x =>
                x.ProcessStep >= ProcessStep.SbomCreation
            )
            .Count();

        var generatedCount = pb.Where(x =>
                x.ProcessStep > ProcessStep.SbomCreation
                || (x.ProcessStep == ProcessStep.SbomCreation
                    && x.ProcessStatus == ProcessStatus.Success)
            )
            .Count();

        var ingestedCount = pb.Where(x =>
                x.ProcessStep > ProcessStep.SbomIngest
                || (x.ProcessStep == ProcessStep.SbomIngest
                    && x.ProcessStatus == ProcessStatus.Success)
            )
            .Count();

        var completeCount = pb.Where(x =>
                x.ProcessStep > ProcessStep.Processed
                || (x.ProcessStep == ProcessStep.Processed
                    && x.ProcessStatus == ProcessStatus.Success)
            )
            .Count();

        return new ProjectBranchDto
        {
            Items =
            [
                .. pb.Select(x => new BranchItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProcessStatus = x.ProcessStatus.ToString(),
                    ProcessStep = x.ProcessStep.ToString(),
                    IsTag = x.IsTag,
                    PackageCount = x.PackageCount,
                    VulnerabilityCount = x.VulnerabilityCount,
                    Commits =
                    [
                        .. x
                            .BranchHistories.OrderByDescending(x => x.CommitDate)
                            .Select(bh => new BranchCommitDto
                            {
                                CommitId = bh.Id,
                                CommitName = bh.CommitMessage,
                                ProcessState = bh.ProcessState,
                                ProcessStatus = bh.ProcessStatus
                            })
                    ]
                })
            ],
            TotalCount = pb.Count,
            Complete = completeCount,
            SbomIngested = ingestedCount,
            Initiated = initiatedCount,
            SbomGenerated = generatedCount
        };
    }

    public static PackageItemDto MapToPackageItemDto(this SbomPackage pb)
    {
        return new PackageItemDto
        {
            Id = pb.Id,
            Name = pb.Name,
            Ecosystem = pb.Ecosystem,
            Version = pb.Version,
            Vulnerable = pb.Vulnerabilities.Count > 0
        };
    }

    public static ProjectBranchDetailedDto MapToBranchesDetailedDto(this ProjectBranch pb)
    {
        return new ProjectBranchDetailedDto
        {
            Id = pb.Id,
            Name = pb.Name,
            PackageCount = pb.PackageCount,
            VulnerabilityCount = pb.VulnerabilityCount,
            CommitDate = pb.CommitDate,
            CommitMessage = pb.CommitMessage,
            CommitSha = pb.CommitSha,
            ScanDate = pb.ScanDate
        };
    }

    public static BranchHistoryDto MapToBranchHistoryDto(this ProjectBranch bh)
    {
        var processingStep = bh.HistoryProcessingStep;

        return new BranchHistoryDto
        {
            Histories =
            [
                .. bh
                    .BranchHistories.OrderBy(x => x.CommitDate)
                    .Select(x => new BranchHistoryEntryDto
                    {
                        CommitDate = x.CommitDate,
                        CommitMessage = x.CommitMessage,
                        CommitSha = x.CommitSha,
                        PackageCount = x.PackageCount,
                        VulnerabilityCount = x.VulnerabilityCount,
                        DirectVulnerabilityCount = x.DirectVulnerabilityCount
                    })
            ],
            ProcessingStep = processingStep
        };
    }
}