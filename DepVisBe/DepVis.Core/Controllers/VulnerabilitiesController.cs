using DepVis.Core.Dtos;
using DepVis.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Controllers;

[Route("api/projects")]
[ApiController]
public class VulnerabilitiesController(VulnerabilityService vulnerabilityService) : ControllerBase
{
    [HttpGet("{branchId}/vulnerabilities")]
    public async Task<ActionResult<VulnerabilitiesDto>> GetVulnerabilities(
        Guid branchId,
        ODataQueryOptions<VulnerabilitySmallDto> odata
    )
    {
        var dto = await vulnerabilityService.GetVulnerabilities(branchId, odata);
        return Ok(dto);
    }

    [HttpGet("vulnerabilities/{vulnId}")]
    public async Task<ActionResult<VulnerabilityDetailedDto>> GetVulnerability(string vulnId)
    {
        var dto = await vulnerabilityService.GetVulnerability(vulnId);
        return dto is null ? NotFound() : Ok(dto);
    }
}
