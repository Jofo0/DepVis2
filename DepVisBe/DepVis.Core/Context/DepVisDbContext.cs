using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Context;

public class DepVisDbContext : DbContext
{
    public DepVisDbContext(DbContextOptions<DepVisDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Sbom> Sboms { get; set; }
    public DbSet<SbomPackage> SbomPackages { get; set; }
}
