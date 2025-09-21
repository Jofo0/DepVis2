using DepVis.Core.Context;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Repositories;

public class ProjectRepository(DepVisDbContext context) : IProjectRepository
{
    public async Task<List<Project>> GetAllAsync() =>
        await context.Projects.AsNoTracking().ToListAsync();

    public Task<Project?> GetByIdAsync(Guid id) =>
        context.Projects.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Project project) => await context.Projects.AddAsync(project);

    public Task UpdateAsync(Project project)
    {
        context.Projects.Update(project);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Project project)
    {
        context.Projects.Remove(project);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
