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
        var sbom = await repo.GetPackagesByIdAndBranch(id);

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
        return (await repo.GetProjectStats(id))?.MapToDto();
    }

    public async Task<List<string>> GetProjectBranches(Guid id)
    {
        return await repo.GetProjectBranches(id);
    }

    public async Task<ProjectDto> CreateProject(CreateProjectDto dto)
    {
        var projectId = Guid.NewGuid();

        var project = new Project
        {
            Id = projectId,
            Name = dto.Name,
            ProjectType = dto.ProjectType,
            ProjectLink = dto.ProjectLink,
        };

        List<ProjectBranches> projectBranches =
        [
            .. dto.Branches.Select(x => new ProjectBranches()
            {
                IsTag = false,
                Name = x,
                ProjectId = projectId,
            }),
        ];

        projectBranches.AddRange(
            dto.Tags.Select(x => new ProjectBranches()
            {
                IsTag = true,
                Name = x,
                ProjectId = projectId,
            })
        );

        if (projectBranches.Count == 0)
        {
            projectBranches.Add(
                new ProjectBranches()
                {
                    IsTag = false,
                    Name = "master",
                    ProjectId = projectId,
                }
            );
        }

        project.ProjectBranches = projectBranches;

        await repo.AddAsync(project);

        // todo maybe move this somewhere else, and just send one message that processing should be started
        foreach (var branch in projectBranches)
        {
            await publishEndpoint.Publish<ProcessingMessage>(
                new()
                {
                    GitHubLink = project.ProjectLink,
                    ProjectBranchId = branch.Id,
                    Branch = branch.Name,
                }
            );
        }

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
