using DepVis.Core.Context;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class ProjectRepository(DepVisDbContext context) : IProjectRepository
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

    // TODO move to PackagesRepo
    public async Task<Sbom?> GetPackagesAndChildrenByIdAndBranch(Guid id) =>
        await context
            .Sboms.AsNoTracking()
            .Where(x => x.ProjectBranchId == id)
            .Include(sboms => sboms.SbomPackages)
            .ThenInclude(sp => sp.Children)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public IQueryable<SbomPackage> GetPackagesForBranch(Guid branchId)
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

    public async Task<Vulnerability?> GetVulnerability(string id) =>
        await context.Vulnerabilities.AsNoTracking().FirstAsync(x => x.Id == id);

    public async Task<ProjectBranches?> GetProjectStats(Guid id) =>
        await context.ProjectBranches.AsNoTracking().FirstAsync(x => x.Id == id);

    public async Task<List<ProjectBranches>> GetProjectBranches(Guid id) =>
        await context.ProjectBranches.AsNoTracking().Where(x => x.ProjectId == id).ToListAsync();

    public IQueryable<ProjectBranches> GetProjectBranchesAsQueryable(Guid id) =>
        context.ProjectBranches.Include(x => x.Sboms).AsNoTracking().Where(x => x.ProjectId == id);

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
}
