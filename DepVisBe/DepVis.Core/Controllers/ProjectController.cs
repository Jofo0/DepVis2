using DepVis.Core.Dtos;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var projects = await service.GetProjects();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
    {
        var project = await service.GetProject(id);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpGet("{id}/branches")]
    public async Task<ActionResult<List<ProjectBranchDto>>> GetProjectBranches(Guid id)
    {
        var projectBranches = await service.GetProjectBranches(id);
        if (projectBranches is null)
            return NotFound();
        return Ok(projectBranches);
    }

    [HttpGet("{id}/branches/detailed")]
    public async Task<ActionResult<List<ProjectBranchDetailedDto>>> GetProjectBranchesTableData(
        Guid id,
        ODataQueryOptions<ProjectBranches> odata
    )
    {
        var projectBranchesDetailed = await service.GetProjectBranchesDetailed(id, odata);
        if (projectBranchesDetailed is null)
            return NotFound();
        return Ok(projectBranchesDetailed);
    }

    [HttpGet("{branchId}/packages")]
    public async Task<ActionResult<List<PackageDetailedDto>>> GetBranchPackages(Guid branchId)
    {
        var project = await service.GetProjectGraphData(branchId);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpGet("{branchId}/packages/graph")]
    public async Task<ActionResult<GraphDataDto>> GetFullGraph(Guid branchId)
    {
        var project = await service.GetProjectGraphData(branchId);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpGet("{branchId}/stats")]
    public async Task<ActionResult<ProjectStatsDto>> GetProjectStats(Guid branchId)
    {
        var project = await service.GetProjectStats(branchId);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto dto)
    {
        var result = await service.CreateProject(dto);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        var ok = await service.UpdateProject(id, dto);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var ok = await service.DeleteProject(id);
        return ok ? NoContent() : NotFound();
    }
}
