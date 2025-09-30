using DepVis.Core.Dtos;

namespace DepVis.Core.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjects();
    Task<ProjectDto?> GetProject(Guid id);
    Task<ProjectDto> CreateProject(CreateProjectDto dto);
    Task<bool> UpdateProject(Guid id, UpdateProjectDto dto);
    Task<ProjectStatsDto> GetProjectStats(Guid id, string branch);
    Task<GraphDataDto> GetProjectGraphData(Guid id, string branch);
    Task<bool> DeleteProject(Guid id);
}
