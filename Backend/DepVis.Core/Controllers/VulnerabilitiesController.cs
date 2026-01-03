using DepVis.Core.Dtos;
using DepVis.Core.Services;
using DepVis.Core.Util;
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
        ODataQueryOptions<VulnerabilitySmallDto> odata,
        [FromQuery(Name = "$export")] bool export = false
    )
    {
        var dto = await vulnerabilityService.GetVulnerabilities(branchId, odata);

        if (export)
        {
            var rows = dto.Vulnerabilities;
            var stream = await CsvExport.WriteToCsvStreamAsync(rows);
            stream.Position = 0;
            return File(stream, "text/csv", $"vulnerabilities-{branchId}-{DateTime.Now}.csv");
        }

        return Ok(dto);
    }

    [HttpGet("vulnerabilities/{vulnId}")]
    public async Task<ActionResult<VulnerabilityDetailedDto>> GetVulnerability(string vulnId)
    {
        var dto = await vulnerabilityService.GetVulnerability(vulnId);
        return dto is null ? NotFound() : Ok(dto);
    }
}
