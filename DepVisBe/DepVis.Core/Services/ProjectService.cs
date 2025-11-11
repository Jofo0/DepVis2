using DepVis.Core.Dtos;
using DepVis.Core.Extensions;
using DepVis.Shared.Messages;
using DepVis.Shared.Model;
using MassTransit;

public class ProjectService(ProjectRepository repo, IPublishEndpoint publishEndpoint)
{
    public async Task<IEnumerable<ProjectDto>> GetProjects() =>
        (await repo.GetAllAsync()).Select(x => x.MapToDto());

    public async Task<ProjectDto?> GetProject(Guid id)
    {
        var project = await repo.GetByIdAsync(id);
        return project is null ? null : project.MapToDto();
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
            ProjectBranches =
            [
                .. dto.Branches.Select(b => new ProjectBranches
                {
                    IsTag = false,
                    Name = b,
                    ProjectId = projectId,
                }),
                .. dto.Tags.Select(t => new ProjectBranches
                {
                    IsTag = true,
                    Name = t,
                    ProjectId = projectId,
                }),
            ],
        };

        if (project.ProjectBranches.Count == 0)
            project.ProjectBranches.Add(
                new ProjectBranches
                {
                    IsTag = false,
                    Name = "master",
                    ProjectId = projectId,
                }
            );

        await repo.AddAsync(project);

        foreach (var branch in project.ProjectBranches)
        {
            await publishEndpoint.Publish<ProcessingMessage>(
                new()
                {
                    GitHubLink = project.ProjectLink,
                    ProjectBranchId = branch.Id,
                    Location = branch.Name,
                    IsTag = branch.IsTag,
                }
            );
        }

        return project.MapToDto();
    }

    public async Task<ProjectStatsDto?> GetProjectStats(Guid branchId) =>
        (await repo.GetProjectStatsAsync(branchId))?.MapToDto();

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
