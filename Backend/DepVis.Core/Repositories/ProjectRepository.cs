using DepVis.Core.Context;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

public class ProjectRepository(DepVisDbContext context)
{
    public async Task<List<Project>> GetAllAsync() =>
        await context.Projects.AsNoTracking().ToListAsync();

    public async Task<Project?> GetByIdAsync(Guid id) =>
        await context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetByIdDetailedAsync(Guid id) =>
        await context
            .Projects.AsNoTracking()
            .Include(x => x.ProjectBranches)
            .ThenInclude(x => x.Sboms)
            .ThenInclude(sboms => sboms.SbomPackages)
            .ThenInclude(sp => sp.Children)
            .ThenInclude(cd => cd.Child)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Project project)
    {
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        context.Projects.Update(project);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        var projectId = project.Id;

        await context
            .PackageDependencies.Where(pd =>
                pd.Parent.Sbom.ProjectBranch.Project.Id == projectId
                || pd.Child.Sbom.ProjectBranch.Project.Id == projectId
            )
            .ExecuteDeleteAsync();

        await context
            .SbomPackageVulnerabilities.Where(pv =>
                pv.SbomPackage.Sbom.ProjectBranch.Project.Id == projectId
            )
            .ExecuteDeleteAsync();

        await context
            .SbomPackages.Where(sp => sp.Sbom.ProjectBranch.Project.Id == projectId)
            .ExecuteDeleteAsync();

        await context
            .Sboms.Where(s => s.ProjectBranch.Project.Id == projectId)
            .ExecuteDeleteAsync();

        await context.ProjectBranches.Where(b => b.Project.Id == projectId).ExecuteDeleteAsync();

        await context.Projects.Where(p => p.Id == projectId).ExecuteDeleteAsync();
    }

    public async Task<ProjectBranch?> GetProjectStatsAsync(Guid branchId) =>
        await context.ProjectBranches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == branchId);
}
