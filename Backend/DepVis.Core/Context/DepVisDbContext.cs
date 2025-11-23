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
    public DbSet<SbomPackageVulnerability> SbomPackageVulnerabilities =>
        Set<SbomPackageVulnerability>();
    public DbSet<ProjectBranches> ProjectBranches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PackageDependency>().HasKey(pd => new { pd.ParentId, pd.ChildId });
        modelBuilder
            .Entity<SbomPackage>()
            .HasMany(p => p.Vulnerabilities)
            .WithMany(v => v.AffectedPackages)
            .UsingEntity<SbomPackageVulnerability>(
                j =>
                    j.HasOne(pv => pv.Vulnerability)
                        .WithMany(v => v.SbomPackageVulnerabilities)
                        .HasForeignKey(pv => pv.VulnerabilityId)
                        .OnDelete(DeleteBehavior.NoAction),
                j =>
                    j.HasOne(pv => pv.SbomPackage)
                        .WithMany(p => p.SbomPackageVulnerabilities)
                        .HasForeignKey(pv => pv.SbomPackageId)
                        .OnDelete(DeleteBehavior.NoAction),
                j =>
                {
                    j.ToTable("SbomPackageVulnerabilities");
                    j.HasKey(pv => new { pv.SbomPackageId, pv.VulnerabilityId });
                    j.HasIndex(pv => pv.VulnerabilityId);
                }
            );

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
