using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdDetailedAsync(Guid id);
    Task<Sbom?> GetPackagesByIdAndBranch(Guid id, string branch);
    Task AddAsync(Project project);
    Task<ProjectStatistics?> GetProjectStats(Guid id, string branch);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
