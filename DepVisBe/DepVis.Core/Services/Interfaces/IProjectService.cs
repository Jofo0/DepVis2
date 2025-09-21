using DepVis.Core.Dtos;

namespace DepVis.Core.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsAsync();
    Task<ProjectDto?> GetProjectAsync(Guid id);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto);
    Task<bool> UpdateProjectAsync(Guid id, UpdateProjectDto dto);
    Task<bool> DeleteProjectAsync(Guid id);
}
