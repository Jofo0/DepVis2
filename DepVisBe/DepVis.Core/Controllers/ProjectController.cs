using DepVis.Core.Context;
using DepVis.Core.Dtos;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(DepVisDbContext context, IPublishEndpoint publishEndpoint)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var projects = await context
            .Projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ProjectType = p.ProjectType,
                ProcessStatus = p.ProcessStatus,
                ProjectLink = p.ProjectLink,
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            ProjectType = project.ProjectType,
            ProcessStatus = project.ProcessStatus,
            ProcessStep = project.ProcessStep,
            ProjectLink = project.ProjectLink,
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            ProjectType = dto.ProjectType,
            ProjectLink = dto.ProjectLink,
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var result = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            ProjectType = project.ProjectType,
            ProcessStatus = project.ProcessStatus,
            ProjectLink = project.ProjectLink,
        };

        await publishEndpoint.Publish<ProcessingMessage>(
            new() { GitHubLink = result.ProjectLink, ProjectId = project.Id }
        );

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.Name = dto.Name;
        project.ProjectType = dto.ProjectType;
        project.ProjectLink = dto.ProjectLink;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        context.Projects.Remove(project);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
