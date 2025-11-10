using DepVis.Core.Dtos;
using DepVis.Core.Services;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Controllers;

[Route("api/projects")]
[ApiController]
public class ProjectBranchesController(
    ProjectBranchService branchService,
    ProjectService projectService
) : ControllerBase
{
    [HttpGet("{id}/branches")]
    public async Task<ActionResult<List<ProjectBranchDto>>> GetProjectBranches(Guid id)
    {
        var branches = await branchService.GetProjectBranches(id);
        return Ok(branches);
    }

    [HttpGet("{id}/branches/detailed")]
    public async Task<ActionResult<List<ProjectBranchDetailedDto>>> GetProjectBranchesDetailed(
        Guid id,
        ODataQueryOptions<ProjectBranches> odata
    )
    {
        var detailed = await branchService.GetProjectBranchesDetailed(id, odata);
        return Ok(detailed);
    }

    [HttpGet("{branchId}/stats")]
    public async Task<ActionResult<ProjectStatsDto>> GetProjectStats(Guid branchId)
    {
        var stats = await projectService.GetProjectStats(branchId);
        return stats is null ? NotFound() : Ok(stats);
    }
}
