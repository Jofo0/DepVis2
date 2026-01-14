using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Services;

public class ProjectBranchService(ProjectBranchRepository repo, IPublishEndpoint publishEndpoint)
{
    public async Task<ProjectBranchDto> GetProjectBranches(Guid id)
    {
        return (await repo.GetByProjectAsync(id)).MapToBranchesDto();
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

        branch.HistoryProcessingStep = Shared.Model.Enums.ProcessStep.Created;
        await repo.Update(branch, cancellationToken);

        await publishEndpoint.Publish(
            new BranchHistoryProcessingMessage()
            {
                GitHubLink = branch.Project.ProjectLink,
                Location = branch.Name,
                ProjectBranchId = branch.Id,
            },
            cancellationToken
        );
    }
}
