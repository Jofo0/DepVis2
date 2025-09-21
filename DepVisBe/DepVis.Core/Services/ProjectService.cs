using DepVis.Core.Dtos;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;

namespace DepVis.Core.Services;

public class ProjectService(IProjectRepository repo, IPublishEndpoint publishEndpoint)
    : IProjectService
{
    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
    {
        var projects = await repo.GetAllAsync();
        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto?> GetProjectAsync(Guid id)
    {
        var project = await repo.GetByIdAsync(id);
        return project is null ? null : MapToDtoWithProcessStep(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            ProjectType = dto.ProjectType,
            ProjectLink = dto.ProjectLink,
        };

        await repo.AddAsync(project);
        await repo.SaveChangesAsync();

        await publishEndpoint.Publish<ProcessingMessage>(
            new()
            {
                GitHubLink = project.ProjectLink,
                ProjectId = project.Id,
                Branch = "master",
            }
        );

        return MapToDto(project);
    }

    public async Task<bool> UpdateProjectAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await repo.GetByIdAsync(id);
        if (project is null)
            return false;

        project.Name = dto.Name;
        project.ProjectType = dto.ProjectType;
        project.ProjectLink = dto.ProjectLink;

        await repo.UpdateAsync(project);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = await repo.GetByIdAsync(id);
        if (project is null)
            return false;

        await repo.DeleteAsync(project);
        await repo.SaveChangesAsync();
        return true;
    }

    // Mapping helpers
    private static ProjectDto MapToDto(Project p) =>
        new()
        {
            Id = p.Id,
            Name = p.Name,
            ProjectType = p.ProjectType,
            ProcessStatus = p.ProcessStatus,
            ProjectLink = p.ProjectLink,
        };

    private static ProjectDto MapToDtoWithProcessStep(Project p)
    {
        var dto = MapToDto(p);
        dto.ProcessStep = p.ProcessStep;
        return dto;
    }
}
