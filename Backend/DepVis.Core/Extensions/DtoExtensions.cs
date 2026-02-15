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

    public static ProjectBranchDto MapToBranchesDto(this List<ProjectBranch> pb)
    {
        var initiatedCount = pb.Where(x =>
                x.ProcessStep >= Shared.Model.Enums.ProcessStep.SbomCreation
            )
            .Count();

        var generatedCount = pb.Where(x =>
                x.ProcessStep > Shared.Model.Enums.ProcessStep.SbomCreation
                || x.ProcessStep == Shared.Model.Enums.ProcessStep.SbomCreation
                    && x.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success
            )
            .Count();

        var ingestedCount = pb.Where(x =>
                x.ProcessStep > Shared.Model.Enums.ProcessStep.SbomIngest
                || x.ProcessStep == Shared.Model.Enums.ProcessStep.SbomIngest
                    && x.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success
            )
            .Count();

        var completeCount = pb.Where(x =>
                x.ProcessStep > Shared.Model.Enums.ProcessStep.Processed
                || x.ProcessStep == Shared.Model.Enums.ProcessStep.Processed
                    && x.ProcessStatus == Shared.Model.Enums.ProcessStatus.Success
            )
            .Count();

        return new ProjectBranchDto()
        {
            Items =
            [
                .. pb.Select(x => new BranchItemDto()
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
                            .Select(x => new BranchCommitDto()
                            {
                                CommitId = x.Id,
                                CommitName = x.CommitMessage,
                            }),
                    ],
                }),
            ],
            TotalCount = pb.Count,
            Complete = completeCount,
            SbomIngested = ingestedCount,
            Initiated = initiatedCount,
            SbomGenerated = generatedCount,
        };
    }

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
                        VulnerabilityCount = x.VulnerabilityCount,
                    }),
            ],
            ProcessingStep = processingStep,
        };
    }
}
