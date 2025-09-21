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
    public DbSet<PackageDependency> PackageDependencies => Set<PackageDependency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PackageDependency>().HasKey(pd => new { pd.ParentId, pd.ChildId });

        modelBuilder
            .Entity<PackageDependency>()
            .HasOne(pd => pd.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(pd => pd.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<PackageDependency>()
            .HasOne(pd => pd.Child)
            .WithMany(p => p.Parents)
            .HasForeignKey(pd => pd.ChildId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
