using DepVis.Core.Context;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class ProjectBranchRepository(DepVisDbContext context)
{
    public async Task<ProjectBranch?> GetByIdAsync(Guid id) =>
        await context
            .ProjectBranches.Include(x => x.Project)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<ProjectBranch>> GetByProjectAsync(Guid projectId) =>
        await context
            .ProjectBranches.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();

    public async Task<ProjectBranch?> GetProjectBranchHistory(
        Guid projectBranchId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .ProjectBranches.AsNoTracking()
            .Where(x => x.Id == projectBranchId)
            .Include(x => x.BranchHistories)
            .FirstOrDefaultAsync(cancellationToken);

    public IQueryable<ProjectBranch> QueryByProject(Guid projectId) =>
        context
            .ProjectBranches.Include(x => x.Sboms)
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId);

    public Task Update(ProjectBranch projectBranch, CancellationToken cancellationToken = default)
    {
        context.ProjectBranches.Update(projectBranch);
        return context.SaveChangesAsync(cancellationToken);
    }
}
