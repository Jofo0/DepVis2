using DepVis.Core.Dtos;
using DepVis.Shared.Model;
using Microsoft.AspNetCore.OData.Query;

namespace DepVis.Core.Services.Interfaces;

public interface IProjectBranchService
{
    Task<ProjectBranchDto> GetProjectBranches(Guid id);
    Task ProcessBranch(Guid id);
    Task IngestHistory(Guid historyId, CancellationToken cancellationToken);
    Task<BranchCompareDto> GetComparison(Guid mainBranch, Guid comparedBranchId);
    Task<List<ProjectBranchDetailedDto>> GetProjectBranchesDetailed(Guid id, ODataQueryOptions<ProjectBranch> odata);
    Task<BranchHistoryDto?> GetBranchHistory(Guid projectBranchId, CancellationToken cancellationToken);
    Task<Sbom?> GetLatestSbomForBranch(Guid branchId, CancellationToken cancellationToken);
    Task ProcessHistory(Guid projectBranchId, CancellationToken cancellationToken);
    BranchProgressDto? GetBranchProgress(Guid branchId);
}