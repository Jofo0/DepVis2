using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using DepVis.Shared.Model.Enums;
using MassTransit;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Services;

public class ProjectBranchService(ProjectBranchRepository repo, IPublishEndpoint publishEndpoint)
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

        await repo.DeleteBranchDependencies(id);
        await publishEndpoint.Publish<ProcessingMessage>(
            new()
            {
                GitHubLink = branch.Project.ProjectLink,
                GitTarget = new()
                {
                    IsTag = branch.IsTag,
                    Location = branch.Name,
                    ProjectBranchId = branch.Id,
                },
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
        var mainBranchData = await repo.GetCompareDataAsync(mainBranch);
        var comparedBranchData = await repo.GetCompareDataAsync(comparedBranchId);

        var branchPackages = mainBranchData.PackageNames;
        var comparedPackages = comparedBranchData.PackageNames;

        var branchPackageSet = new HashSet<SmallPackage>(mainBranchData.PackageNames);
        var comparedPackageSet = new HashSet<SmallPackage>(comparedBranchData.PackageNames);

        var removedPackages = mainBranchData
            .PackageNames.Where(p => !comparedPackageSet.Contains(p))
            .ToList();

        var addedPackages = comparedBranchData
            .PackageNames.Where(p => !branchPackageSet.Contains(p))
            .ToList();

        var sourceVulnIds = new HashSet<string>(mainBranchData.VulnerabilityIds);
        var targetVulnIds = new HashSet<string>(comparedBranchData.VulnerabilityIds);

        var removedVulnerabilityIds = sourceVulnIds.Except(targetVulnIds).ToList();
        var addedVulnerabilityIds = targetVulnIds.Except(sourceVulnIds).ToList();

        return new BranchCompareDto(
            [.. addedPackages.Select(x => x.Name)],
            [.. removedPackages.Select(x => x.Name)],
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
        return data?.MapToBranchHistoryDto();
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
}