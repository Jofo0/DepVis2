using DepVis.Core.Context;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class PackageRepository(DepVisDbContext context)
{
    public IQueryable<SbomPackage> GetLatestPackagesForBranch(Guid branchId)
    {
        var latestSbomIdQuery = context
            .Sboms.Where(s => s.ProjectBranchId == branchId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => s.Id)
            .Take(1);

        return context
            .SbomPackages.Where(p => latestSbomIdQuery.Contains(p.SbomId))
            .Include(x => x.Vulnerabilities)
            .AsNoTracking();
    }

    public async Task<SbomPackage?> GetPackage(Guid packageId, CancellationToken cancellation)
    {
        return await context
            .SbomPackages.Where(p => p.Id == packageId)
            .Include(x => x.Vulnerabilities)
            .AsNoTracking()
            .FirstAsync(cancellation);
    }
}
