using DepVis.Core.Dtos;
using DepVis.Core.Services;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Controllers;

[Route("api/projects")]
[ApiController]
public class PackagesController(PackageService packageService, GraphService graphService)
    : ControllerBase
{
    [HttpGet("{branchId}/packages")]
    public async Task<ActionResult<PackageDetailedDto>> GetBranchPackages(
        Guid branchId,
        ODataQueryOptions<SbomPackage> odata
    )
    {
        var dto = await packageService.GetPackageData(branchId, odata);
        return Ok(dto);
    }

    [HttpGet("{branchId}/packages/graph")]
    public async Task<ActionResult<GraphDataDto>> GetFullGraph(
        Guid branchId,
        [FromQuery] string? severity,
        [FromQuery] bool showAllParents = true
    )
    {
        var graph = await graphService.GetProjectGraphData(branchId, showAllParents, severity);
        return graph is null ? NotFound() : Ok(graph);
    }

    [HttpGet("{branchId}/packages/graph/{packageId}")]
    public async Task<ActionResult<GraphDataDto>> GetPackageHierarchyGraphData(
        Guid branchId,
        Guid packageId
    )
    {
        var graph = await graphService.GetPackageHierarchyGraphData(branchId, packageId);
        return graph is null ? NotFound() : Ok(graph);
    }
}
