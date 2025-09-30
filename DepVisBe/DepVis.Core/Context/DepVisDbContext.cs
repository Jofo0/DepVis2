using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Context;

public class DepVisDbContext : DbContext
{
    public DepVisDbContext(DbContextOptions<DepVisDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Sbom> Sboms { get; set; }
    public DbSet<ProjectStatistics> ProjectStatistics { get; set; }
    public DbSet<SbomPackage> SbomPackages { get; set; }
    public DbSet<PackageDependency> PackageDependencies => Set<PackageDependency>();
    public DbSet<Vulnerability> Vulnerabilities { get; set; }
    public DbSet<PackageVulnerability> PackageVulnerabilities => Set<PackageVulnerability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PackageDependency>().HasKey(pd => new { pd.ParentId, pd.ChildId });
        modelBuilder
            .Entity<PackageVulnerability>()
            .HasKey(pv => new { pv.SbomPackageId, pv.VulnerabilityId });

        modelBuilder
            .Entity<PackageDependency>()
            .HasOne(pd => pd.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(pd => pd.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<PackageDependency>()
            .HasOne(pd => pd.Child)
            .WithMany(p => p.Parents)
            .HasForeignKey(pd => pd.ChildId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
