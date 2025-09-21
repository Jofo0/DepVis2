using DepVis.Core.Dtos;
using DepVis.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DepVis.Core.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var projects = await service.GetProjectsAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
    {
        var project = await service.GetProjectAsync(id);
        if (project is null)
            return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto dto)
    {
        var result = await service.CreateProjectAsync(dto);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        var ok = await service.UpdateProjectAsync(id, dto);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var ok = await service.DeleteProjectAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
