using DepVis.Core.Context;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class SbomRepository(DepVisDbContext context)
{
    public async Task<Sbom?> GetLatestWithPackagesAndChildrenAsync(Guid id) =>
        await context
            .Sboms.AsNoTracking()
            .Where(x => x.ProjectBranchId == id || x.BranchHistoryId == id)
            .Include(x => x.SbomPackages)
            .ThenInclude(x => x.Children)
            .Include(x => x.SbomPackages)
            .ThenInclude(x => x.Parents)
            .ThenInclude(x => x.Parent)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Sbom?> GetLatestWithPackagesAndParentsAsync(Guid id) =>
        await context
            .Sboms.AsNoTracking()
            .Where(x => x.ProjectBranchId == id || x.BranchHistoryId == id)
            .Include(x => x.SbomPackages)
            .ThenInclude(sp => sp.Parents)
            .ThenInclude(p => p.Parent)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
}
