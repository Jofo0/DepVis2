using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdDetailedAsync(Guid id);
    Task<Sbom?> GetPackagesAndChildrenByIdAndBranch(Guid id);
    Task AddAsync(Project project);
    Task<Vulnerability?> GetVulnerability(string id);
    Task<ProjectBranches?> GetProjectStats(Guid id);
    Task<List<ProjectBranches>> GetProjectBranches(Guid id);
    Task<Sbom?> GetPackagesAndParentsByIdAndBranch(Guid id);
    IQueryable<SbomPackage> GetPackagesForBranch(Guid branchId);
    IQueryable<ProjectBranches> GetProjectBranchesAsQueryable(Guid id);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
