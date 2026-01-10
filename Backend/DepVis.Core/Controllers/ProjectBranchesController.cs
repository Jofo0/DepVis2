using DepVis.Core.Dtos;
using DepVis.Core.Services;
using DepVis.Core.Util;
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
    [HttpGet("{branchId}/branches")]
    public async Task<ActionResult<List<ProjectBranchDto>>> GetProjectBranches(Guid branchId)
    {
        var branches = await branchService.GetProjectBranches(branchId);
        return Ok(branches);
    }

    [HttpGet("{branchId}/branches/detailed")]
    public async Task<ActionResult<List<ProjectBranchDetailedDto>>> GetProjectBranchesDetailed(
        Guid branchId,
        ODataQueryOptions<ProjectBranch> odata,
        [FromQuery(Name = "$export")] bool export = false
    )
    {
        var detailed = await branchService.GetProjectBranchesDetailed(branchId, odata);

        if (export)
        {
            var rows = detailed;
            var stream = await CsvExport.WriteToCsvStreamAsync(rows);
            stream.Position = 0;
            return File(stream, "text/csv", $"branches-{branchId}-{DateTime.Now}.csv");
        }

        return Ok(detailed);
    }

    [HttpGet("{branchId}/stats")]
    public async Task<ActionResult<ProjectStatsDto>> GetProjectStats(Guid branchId)
    {
        var stats = await projectService.GetProjectStats(branchId);
        return stats is null ? NotFound() : Ok(stats);
    }

    [HttpPost("{branchId}/history")]
    public async Task<ActionResult<ProjectStatsDto>> ProcessBranchHistory(
        Guid branchId,
        CancellationToken cancellationToken
    )
    {
        await branchService.ProcessHistory(branchId, cancellationToken);
        return Ok();
    }
}
