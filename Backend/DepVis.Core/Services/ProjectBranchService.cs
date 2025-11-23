using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Services;

public class ProjectBranchService(ProjectBranchRepository repo)
{
    public async Task<List<ProjectBranchDto>> GetProjectBranches(Guid id) =>
        [.. (await repo.GetByProjectAsync(id)).Select(x => x.MapToBranchesDto())];

    public async Task<List<ProjectBranchDetailedDto>> GetProjectBranchesDetailed(
        Guid id,
        ODataQueryOptions<ProjectBranches> odata
    )
    {
        var data = await odata.ApplyOdata(repo.QueryByProject(id));
        return [.. data.Select(x => x.MapToBranchesDetailedDto())];
    }
}
