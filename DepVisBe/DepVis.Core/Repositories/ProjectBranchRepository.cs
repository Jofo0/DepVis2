using DepVis.Core.Context;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class ProjectBranchRepository(DepVisDbContext context)
{
    public async Task<ProjectBranches?> GetByIdAsync(Guid id) =>
        await context.ProjectBranches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<ProjectBranches>> GetByProjectAsync(Guid projectId) =>
        await context
            .ProjectBranches.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();

    public IQueryable<ProjectBranches> QueryByProject(Guid projectId) =>
        context
            .ProjectBranches.Include(x => x.Sboms)
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId);
}
