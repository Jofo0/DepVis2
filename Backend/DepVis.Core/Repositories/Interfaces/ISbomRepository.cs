using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface ISbomRepository
{
    Task<Sbom?> GetLatestWithPackagesAndChildrenAsync(Guid id);
    Task<Sbom?> GetByHistoryIdAsync(Guid historyId);
    Task<Sbom?> GetLatestByBranchIdAsync(Guid branchId);
    Task<Sbom?> GetLatestWithPackagesAndParentsAsync(Guid id);
}

