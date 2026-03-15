using DepVis.Core.Context;
using DepVis.Shared.Model;
using DepVis.Shared.Services;
using Microsoft.EntityFrameworkCore;

public class ProjectRepository(DepVisDbContext context, MinioStorageService minio)
{
    public async Task<List<Project>> GetAllAsync() =>
        await context.Projects.Include(x => x.ProjectStatistics).AsNoTracking().ToListAsync();

    public async Task<Project?> GetByIdAsync(Guid id) =>
        await context
            .Projects.Include(x => x.ProjectBranches)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

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

    public async Task RemoveBranchesAsync(Guid projectId, List<string> branches)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();

                var packageDependencies = context.PackageDependencies.Where(pd =>
                    (
                        pd.Parent.Sbom.ProjectBranch != null
                        && branches.Contains(pd.Parent.Sbom.ProjectBranch.Name)
                        && pd.Parent.Sbom.ProjectBranch.Project.Id == projectId
                    )
                    || (
                        pd.Child.Sbom.ProjectBranch != null
                        && branches.Contains(pd.Child.Sbom.ProjectBranch.Name)
                        && pd.Child.Sbom.ProjectBranch.Project.Id == projectId
                    )
                );
                await packageDependencies.ExecuteDeleteAsync();

                var sbomPackageVulnerabilities = context.SbomPackageVulnerabilities.Where(pv =>
                    pv.SbomPackage.Sbom.ProjectBranch != null
                    && branches.Contains(pv.SbomPackage.Sbom.ProjectBranch.Name)
                    && pv.SbomPackage.Sbom.ProjectBranch.Project.Id == projectId
                );
                await sbomPackageVulnerabilities.ExecuteDeleteAsync();

                var sbomPackages = context.SbomPackages.Where(sp =>
                    sp.Sbom.ProjectBranch != null
                    && branches.Contains(sp.Sbom.ProjectBranch.Name)
                    && sp.Sbom.ProjectBranch.Project.Id == projectId
                );
                await sbomPackages.ExecuteDeleteAsync();

                var sboms = context.Sboms.Where(s =>
                    s.ProjectBranch != null
                    && branches.Contains(s.ProjectBranch.Name)
                    && s.ProjectBranch.Project.Id == projectId
                );

                foreach (var sbom in await sboms.ToListAsync())
                {
                    await minio.DeleteAsync(sbom.FileName);
                }

                await sboms.ExecuteDeleteAsync();

                var projectBranches = context.ProjectBranches.Where(b =>
                    b.Project.Id == projectId && branches.Contains(b.Name)
                );
                await projectBranches.ExecuteDeleteAsync();

                await transaction.CommitAsync();
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error deleting branches and associated data.", ex);
        }
    }

    public async Task AddBranchesAsync(List<ProjectBranch> branches)
    {
        await context.ProjectBranches.AddRangeAsync(branches);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        var projectId = project.Id;
        var strategy = context.Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();
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

                foreach (var sbom in await sboms.ToListAsync())
                {
                    await minio.DeleteAsync(sbom.FileName);
                }

                await sboms.ExecuteDeleteAsync();

                var projectBranches = context.ProjectBranches.Where(b => b.Project.Id == projectId);
                await projectBranches.ExecuteDeleteAsync();

                var projectEntity = context.Projects.Where(p => p.Id == projectId);
                await projectEntity.ExecuteDeleteAsync();

                var vulnerabilities = context
                    .Vulnerabilities.Include(x => x.References)
                    .Include(x => x.CWES)
                    .Where(v => !v.SbomPackageVulnerabilities.Any())
                    .ToList();

                foreach (var vulnerability in vulnerabilities)
                {
                    var cwes = vulnerability.CWES.ToList();
                    context.CWEs.RemoveRange(cwes);
                }

                foreach (var vulnerability in vulnerabilities)
                {
                    var references = vulnerability.References.ToList();
                    context.References.RemoveRange(references);
                }

                context.Vulnerabilities.RemoveRange(vulnerabilities);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error deleting project and associated data.", ex);
        }
    }

    public async Task<ProjectBranch?> GetProjectStatsAsync(Guid branchId) =>
        await context.ProjectBranches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == branchId);
}
