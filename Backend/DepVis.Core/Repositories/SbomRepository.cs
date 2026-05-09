using DepVis.Core.Context;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class SbomRepository(DepVisDbContext context) : ISbomRepository
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

    public async Task<Sbom?> GetByHistoryIdAsync(Guid historyId) => await context.Sboms.FirstOrDefaultAsync(x => x.BranchHistoryId == historyId);

    public async Task<Sbom?> GetLatestByBranchIdAsync(Guid branchId) =>
        await context.Sboms
            .AsNoTracking()
            .Where(x => x.ProjectBranchId == branchId)
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
