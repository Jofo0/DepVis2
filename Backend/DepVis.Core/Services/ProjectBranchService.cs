using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Model.Enums;
using MassTransit;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Caching.Memory;

namespace DepVis.Core.Services;

public class ProjectBranchService(
    IProjectBranchRepository repo,
    ISbomRepository sbomRepo,
    IPublishEndpoint publishEndpoint,
    IMemoryCache cache) : IProjectBranchService
{
    public async Task<ProjectBranchDto> GetProjectBranches(Guid id)
    {
        return (await repo.GetByProjectAsync(id)).MapToBranchesDto();
    }

    public async Task ProcessBranch(Guid id)
    {
        var branch = await repo.GetByIdAsync(id);

        if (branch == null)
            return;

        await repo.ResetProjectBranch(id);
        await publishEndpoint.Publish(
            new ProcessingMessage
            {
                GitHubLink = branch.Project.ProjectLink,
                GitTarget = new GitTarget
                {
                    IsTag = branch.IsTag,
                    Location = branch.Name,
                    ProjectBranchId = branch.Id
                }
            }
        );
    }

    public async Task IngestHistory(Guid historyId, CancellationToken cancellationToken)
    {
        var branchHistory = await repo.GetBranchHistoryAsync(historyId, cancellationToken);
        if (branchHistory == null)
            return;

        branchHistory.ProcessState = HistoryProcessing.Ingesting;
        branchHistory.ProcessStatus = ProcessStatus.Pending;

        await repo.Update(branchHistory, cancellationToken);

        await publishEndpoint.Publish(
            new IngestBranchHistoryMessage
            {
                BranchHistoryId = historyId
            },
            cancellationToken
        );
    }

    public async Task<BranchCompareDto> GetComparison(Guid mainBranch, Guid comparedBranchId)
    {
        var (_, branchPackages, vulnerabilityIds) = await repo.GetCompareDataAsync(mainBranch);
        var (_, comparedPackages, list) = await repo.GetCompareDataAsync(comparedBranchId);

        var branchPackageSet = new HashSet<SmallPackage>(branchPackages);
        var comparedPackageSet = new HashSet<SmallPackage>(comparedPackages);

        var removedPackages = branchPackages.Where(p => !comparedPackageSet.Contains(p))
            .ToList();

        var addedPackages = comparedPackages.Where(p => !branchPackageSet.Contains(p))
            .ToList();

        var sourceVulnIds = new HashSet<string>(vulnerabilityIds);
        var targetVulnIds = new HashSet<string>(list);

        var removedVulnerabilityIds = sourceVulnIds.Except(targetVulnIds).ToList();
        var addedVulnerabilityIds = targetVulnIds.Except(sourceVulnIds).ToList();

        return new BranchCompareDto(
            [
                .. addedPackages.Select(x => new PackageInfoDto(
                    x.Name,
                    x.Version,
                    true
                ))
            ],
            [
                .. removedPackages.Select(x => new PackageInfoDto(
                    x.Name,
                    x.Version,
                    true
                ))
            ],
            [
                .. addedPackages
                    .Select(x => x.Ecosystem)
                    .GroupBy(x => x)
                    .Select(g => new NameCount { Name = g.Key, Count = g.Count() })
            ],
            [
                .. removedPackages
                    .Select(x => x.Ecosystem)
                    .GroupBy(x => x)
                    .Select(g => new NameCount { Name = g.Key, Count = g.Count() })
            ],
            addedVulnerabilityIds,
            removedVulnerabilityIds,
            branchPackages.Count,
            comparedPackages.Count,
            sourceVulnIds.Count,
            targetVulnIds.Count
        );
    }

    public async Task<List<ProjectBranchDetailedDto>> GetProjectBranchesDetailed(
        Guid id,
        ODataQueryOptions<ProjectBranch> odata
    )
    {
        var data = await odata.ApplyOdata(repo.QueryByProject(id));
        return [.. data.Select(x => x.MapToBranchesDetailedDto())];
    }

    public async Task<BranchHistoryDto?> GetBranchHistory(
        Guid projectBranchId,
        CancellationToken cancellationToken
    )
    {
        var data = await repo.GetProjectBranchHistory(projectBranchId, cancellationToken);
        var dto = data?.MapToBranchHistoryDto();

        if (dto?.ProcessingStep == ProcessStep.SbomCreation)
        {
            var progress = GetBranchProgress(projectBranchId);
            if (progress != null)
            {
                dto.ProcessedCommits = progress.ProcessedCommits;
                dto.TotalCommits = progress.TotalCommits;
                dto.EstimatedSecondsRemaining = progress.EstimatedSecondsRemaining;
            }
        }

        return dto;
    }

    public async Task<Sbom?> GetLatestSbomForBranch(Guid branchId, CancellationToken cancellationToken)
    {
        return await sbomRepo.GetLatestByBranchIdAsync(branchId);
    }

    public async Task ProcessHistory(Guid projectBranchId, CancellationToken cancellationToken)
    {
        var branch = await repo.GetByIdAsync(projectBranchId);
        if (branch is null)
            return;

        branch.HistoryProcessingStep = ProcessStep.Created;
        await repo.Update(branch, cancellationToken);

        await publishEndpoint.Publish(
            new BranchHistoryProcessingMessage
            {
                GitHubLink = branch.Project.ProjectLink,
                Location = branch.Name,
                ProjectBranchId = branch.Id
            },
            cancellationToken
        );
    }

    public BranchProgressDto? GetBranchProgress(Guid branchId)
    {
        cache.TryGetValue($"branch-progress:{branchId}", out BranchProgressDto? progress);
        return progress;
    }
}