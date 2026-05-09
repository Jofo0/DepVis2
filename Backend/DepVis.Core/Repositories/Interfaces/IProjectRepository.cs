using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdDetailedAsync(Guid id);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task RemoveBranchesAsync(Guid projectId, List<string> branches);
    Task AddBranchesAsync(List<ProjectBranch> branches);
    Task DeleteAsync(Project project);
    Task<ProjectBranch?> GetProjectStatsAsync(Guid branchId);
}
