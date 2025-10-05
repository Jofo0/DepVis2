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
        await context.Projects.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetByIdDetailedAsync(Guid id) =>
        await context
            .Projects.Include(x => x.ProjectBranches)
            .ThenInclude(x => x.Sboms)
            .ThenInclude(sboms => sboms.SbomPackages)
            .ThenInclude(sp => sp.Children)
            .ThenInclude(cd => cd.Child)
            .FirstOrDefaultAsync(p => p.Id == id);

    // TODO move to PackagesRepo
    public async Task<Sbom?> GetPackagesByIdAndBranch(Guid id) =>
        await context
            .Sboms.Where(x => x.ProjectBranchId == id)
            .Include(sboms => sboms.SbomPackages)
            .ThenInclude(sp => sp.Children)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<ProjectBranches?> GetProjectStats(Guid id) =>
        await context.ProjectBranches.FirstAsync(x => x.Id == id);

    public async Task<List<ProjectBranches>> GetProjectBranches(Guid id) =>
        await context.ProjectBranches.Where(x => x.ProjectId == id).ToListAsync();

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
        foreach (var projectBranch in project.ProjectBranches)
        {
            foreach (var sbom in projectBranch.Sboms)
            {
                foreach (var package in sbom.SbomPackages)
                {
                    context.PackageDependencies.RemoveRange(package.Children);
                    context.PackageDependencies.RemoveRange(package.Parents);
                }
            }
            context.Sboms.RemoveRange(projectBranch.Sboms);
        }
        await context.SaveChangesAsync();
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }
}
