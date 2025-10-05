using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdDetailedAsync(Guid id);
    Task<Sbom?> GetPackagesByIdAndBranch(Guid id);
    Task AddAsync(Project project);
    Task<ProjectBranches?> GetProjectStats(Guid id);
    Task<List<ProjectBranches>> GetProjectBranches(Guid id);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
