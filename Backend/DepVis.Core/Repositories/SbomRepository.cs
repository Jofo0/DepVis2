using DepVis.Core.Context;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class SbomRepository(DepVisDbContext context)
{
    public async Task<Sbom?> GetLatestWithPackagesAndChildrenAsync(Guid branchId) =>
        await context
            .Sboms.AsNoTracking()
            .Where(x => x.ProjectBranchId == branchId)
            .Include(x => x.SbomPackages)
            .ThenInclude(x => x.Children)
            .Include(x => x.SbomPackages)
            .ThenInclude(x => x.Parents)
            .ThenInclude(x => x.Parent)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Sbom?> GetLatestWithPackagesAndParentsAsync(Guid branchId) =>
        await context
            .Sboms.AsNoTracking()
            .Where(x => x.ProjectBranchId == branchId)
            .Include(x => x.SbomPackages)
            .ThenInclude(sp => sp.Parents)
            .ThenInclude(p => p.Parent)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
}
