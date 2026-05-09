using DepVis.Core.Dtos;
using DepVis.Core.Services.Interfaces;
using DepVis.Core.Util;
using DepVis.Shared.Model;
using DepVis.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Controllers;

[Route("api/projects")]
[ApiController]
public class ProjectBranchesController(
    IProjectBranchService branchService,
    IProjectService projectService,
    MinioStorageService minioStorageService
) : ControllerBase
{
    [HttpGet("branches")]
    public async Task<ActionResult<ProjectBranchDto>> GetProjectBranches(Guid projectId)
    {
        var branches = await branchService.GetProjectBranches(projectId);
        return Ok(branches);
    }

    [HttpPost("branches/{projectBranchId}/process")]
    public async Task<ActionResult<ProjectBranchDto>> ProcessBranch(Guid projectBranchId)
    {
        await branchService.ProcessBranch(projectBranchId);
        return Ok();
    }

    [HttpGet("branches/detailed")]
    public async Task<ActionResult<List<ProjectBranchDetailedDto>>> GetProjectBranchesDetailed(
        Guid projectId,
        ODataQueryOptions<ProjectBranch> odata,
        [FromQuery(Name = "$export")] bool export = false
    )
    {
        var detailed = await branchService.GetProjectBranchesDetailed(projectId, odata);

        if (export)
        {
            var rows = detailed;
            var stream = await CsvExport.WriteToCsvStreamAsync(rows);
            stream.Position = 0;
            return File(stream, "text/csv", $"branches-{projectId}-{DateTime.UtcNow}.csv");
        }

        return Ok(detailed);
    }

    [HttpGet("branches/{branchId}/compare/{comparedWith}")]
    public async Task<ActionResult<BranchCompareDto>> GetBranchComparison(
        Guid branchId,
        Guid comparedWith
    )
    {
        var stats = await branchService.GetComparison(branchId, comparedWith);
        return stats is null ? NotFound() : Ok(stats);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ProjectStatsDto>> GetProjectStats(Guid projectId)
    {
        var stats = await projectService.GetProjectStats(projectId);
        return stats is null ? NotFound() : Ok(stats);
    }

    [HttpGet("branches/{branchId}/history")]
    public async Task<ActionResult<BranchHistoryDto>> GetBranchHistory(
        Guid branchId,
        CancellationToken cancellationToken,
        [FromQuery(Name = "$export")] bool export = false
    )
    {
        var data = await branchService.GetBranchHistory(branchId, cancellationToken);

        if (data is null) return NotFound();

        if (export)
        {
            var rows = data.Histories;
            var stream = await CsvExport.WriteToCsvStreamAsync(rows);
            stream.Position = 0;
            return File(stream, "text/csv", $"branch-history-{branchId}-{DateTime.UtcNow}.csv");
        }

        return Ok(data);
    }

    [HttpPost("branches/{branchId}/history")]
    public async Task<ActionResult> ProcessBranchHistory(
        Guid branchId,
        CancellationToken cancellationToken
    )
    {
        await branchService.ProcessHistory(branchId, cancellationToken);
        return Ok();
    }

    [HttpPost("branches/{branchId}/history/{branchHistoryId}/ingest")]
    public async Task<ActionResult> IngestBranchHistory(
        Guid branchId,
        Guid branchHistoryId,
        CancellationToken cancellationToken
    )
    {
        await branchService.IngestHistory(branchHistoryId, cancellationToken);
        return Ok();
    }

    [HttpGet("branches/{branchId}/sbom/download")]
    public async Task<IActionResult> DownloadSbom(
        Guid branchId,
        CancellationToken cancellationToken
    )
    {
        var sbom = await branchService.GetLatestSbomForBranch(branchId, cancellationToken);
        if (sbom is null)
            return NotFound();

        var stream = await minioStorageService.RetrieveAsync(sbom.FileName, cancellationToken);
        return File(stream, "application/json", sbom.FileName);
    }
}
