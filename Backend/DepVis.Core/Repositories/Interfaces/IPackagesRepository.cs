using DepVis.Shared.Model;

namespace DepVis.Core.Repositories.Interfaces;

public interface IPackageRepository
{
    IQueryable<SbomPackage> GetLatestPackagesForBranchOrCommit(Guid branchId);
    Task<SbomPackage?> GetPackage(Guid packageId, CancellationToken cancellation);
}
