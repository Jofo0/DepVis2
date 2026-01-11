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

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var packageDependencies = context.PackageDependencies.Where(pd =>
                (
                    pd.Parent.Sbom.ProjectBranch != null
                    && pd.Parent.Sbom.ProjectBranch.Project.Id == projectId
                )
                || (
                    pd.Child.Sbom.ProjectBranch != null
                    && pd.Child.Sbom.ProjectBranch.Project.Id == projectId
                )
            );
            await packageDependencies.ExecuteDeleteAsync();

            var sbomPackageVulnerabilities = context.SbomPackageVulnerabilities.Where(pv =>
                pv.SbomPackage.Sbom.ProjectBranch != null
                && pv.SbomPackage.Sbom.ProjectBranch.Project.Id == projectId
            );
            await sbomPackageVulnerabilities.ExecuteDeleteAsync();

            var sbomPackages = context.SbomPackages.Where(sp =>
                sp.Sbom.ProjectBranch != null && sp.Sbom.ProjectBranch.Project.Id == projectId
            );
            await sbomPackages.ExecuteDeleteAsync();

            var sboms = context.Sboms.Where(s =>
                s.ProjectBranch != null && s.ProjectBranch.Project.Id == projectId
            );
            await sboms.ExecuteDeleteAsync();

            var projectBranches = context.ProjectBranches.Where(b => b.Project.Id == projectId);
            await projectBranches.ExecuteDeleteAsync();

            var projectEntity = context.Projects.Where(p => p.Id == projectId);
            await projectEntity.ExecuteDeleteAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Error deleting project and associated data.", ex);
        }
    }

    public async Task<ProjectBranch?> GetProjectStatsAsync(Guid branchId) =>
        await context.ProjectBranches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == branchId);
}
