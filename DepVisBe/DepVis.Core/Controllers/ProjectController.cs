using DepVis.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DepVis.Core.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(ProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects() =>
        Ok(await service.GetProjects());

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
    {
        var project = await service.GetProject(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto dto)
    {
        var result = await service.CreateProject(dto);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto) =>
        (await service.UpdateProject(id, dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id) =>
        (await service.DeleteProject(id)) ? NoContent() : NotFound();
}
