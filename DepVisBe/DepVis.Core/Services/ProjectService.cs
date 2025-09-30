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
    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
    {
        var projects = await repo.GetAllAsync();
        return projects.Select(x => x.MapToDto());
    }

    public async Task<ProjectDto?> GetProjectAsync(Guid id)
    {
        var project = await repo.GetByIdAsync(id);
        return project is null ? null : project.MapToDto();
    }

    public async Task<GraphDataDto?> GetProjectGraphData(Guid id, string branch = "master")
    {
        var sbom = await repo.GetPackagesByIdAndBranch(id, branch);
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

        return sbom is null
            ? null
            : new GraphDataDto { Packages = packages, Relationships = relations };
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

    public async Task<bool> UpdateProjectAsync(Guid id, UpdateProjectDto dto)
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

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = await repo.GetByIdDetailedAsync(id);
        if (project is null)
            return false;

        await repo.DeleteAsync(project);
        return true;
    }
}
