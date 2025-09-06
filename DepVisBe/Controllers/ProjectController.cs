using DepVisBe.Dtos;
using DepVisBe.Model;
using DepVisBe.Model.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DepVisBe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController(DepVisDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var projects = await context.Projects
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ProjectType = p.ProjectType.ToString(),
                ProcessStatus = p.ProcessStatus.ToString(),
                FolderPath = p.FolderPath,
                GitHubLink = p.GitHubLink
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
            ProjectType = project.ProjectType.ToString(),
            ProcessStatus = project.ProcessStatus.ToString(),
            FolderPath = project.FolderPath,
            GitHubLink = project.GitHubLink
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            ProjectType = dto.ProjectType,
            FolderPath = dto.FolderPath,
            GitHubLink = dto.GitHubLink
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var result = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            ProjectType = project.ProjectType.ToString(),
            ProcessStatus = project.ProcessStatus.ToString(),
            FolderPath = project.FolderPath,
            GitHubLink = project.GitHubLink
        };

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
        project.ProcessStatus = dto.ProcessStatus;
        project.FolderPath = dto.FolderPath;
        project.GitHubLink = dto.GitHubLink;

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
