using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services.Interfaces;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;

namespace DepVis.Core.Services;

public class ProjectService(IProjectRepository repo, IPublishEndpoint publishEndpoint)
    : IProjectService
{
    public async Task<IEnumerable<ProjectDto>> GetProjects()
    {
        var projects = await repo.GetAllAsync();
        return projects.Select(x => x.MapToDto());
    }

    public async Task<ProjectDto?> GetProject(Guid id)
    {
        var project = await repo.GetByIdAsync(id);
        return project is null ? null : project.MapToDto();
    }

    // TODO: Move to different package service
    public async Task<GraphDataDto?> GetProjectGraphData(Guid id, string branch = "master")
    {
        var sbom = await repo.GetPackagesByIdAndBranch(id, branch);

        if (sbom == null)
            return null;

        var relations = sbom
            .SbomPackages.SelectMany(pkg =>
                pkg.Children.Select(child => new PackageRelationDto
                {
                    To = child.ChildId,
                    From = child.ParentId,
                })
            )
            .ToList();

        var packages = sbom
            .SbomPackages.Select(x => new PackageDto { Name = x.Name, Id = x.Id })
            .ToList();

        return new GraphDataDto { Packages = packages, Relationships = relations };
    }

    // TODO: Move to different service

    public async Task<ProjectStatsDto?> GetProjectStats(Guid id, string branch = "master")
    {
        return (await repo.GetProjectStats(id, branch))?.MapToDto();
    }

    public async Task<ProjectDto> CreateProject(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            ProjectType = dto.ProjectType,
            ProjectLink = dto.ProjectLink,
        };

        await repo.AddAsync(project);

        await publishEndpoint.Publish<ProcessingMessage>(
            new()
            {
                GitHubLink = project.ProjectLink,
                ProjectId = project.Id,
                Branch = "master",
            }
        );

        return project.MapToDto();
    }

    public async Task<bool> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        var project = await repo.GetByIdAsync(id);
        if (project is null)
            return false;

        project.Name = dto.Name;
        project.ProjectType = dto.ProjectType;
        project.ProjectLink = dto.ProjectLink;

        await repo.UpdateAsync(project);
        return true;
    }

    public async Task<bool> DeleteProject(Guid id)
    {
        var project = await repo.GetByIdDetailedAsync(id);
        if (project is null)
            return false;

        await repo.DeleteAsync(project);
        return true;
    }
}
