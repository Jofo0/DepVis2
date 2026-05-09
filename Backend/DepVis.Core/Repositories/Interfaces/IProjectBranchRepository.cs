using DepVis.Core.Dtos;
using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface IProjectBranchRepository
{
    Task<ProjectBranch?> GetByIdAsync(Guid id);
    Task<BranchCompareDataModel> GetCompareDataAsync(Guid id);
    Task<List<ProjectBranch>> GetByProjectAsync(Guid projectId);
    Task<ProjectBranch?> GetProjectBranchHistory(Guid projectBranchId, CancellationToken cancellationToken = default);
    IQueryable<ProjectBranch> QueryByProject(Guid projectId);
    Task ResetProjectBranch(Guid projectBranchId);
    Task Update(ProjectBranch projectBranch, CancellationToken cancellationToken = default);
    Task Update(BranchHistory branchHistory, CancellationToken cancellationToken = default);
    Task<BranchHistory?> GetBranchHistoryAsync(Guid historyId, CancellationToken cancellationToken = default);
}

