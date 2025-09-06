using Microsoft.EntityFrameworkCore;

namespace DepVisBe.Model.Context;

public class DepVisDbContext : DbContext
{
    public DepVisDbContext(DbContextOptions<DepVisDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
}
