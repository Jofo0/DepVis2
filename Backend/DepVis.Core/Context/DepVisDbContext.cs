using DepVis.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Context;

public class DepVisDbContext : DbContext
{
    public DepVisDbContext(DbContextOptions<DepVisDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Sbom> Sboms { get; set; }
    public DbSet<CWE> CWEs { get; set; }
    public DbSet<ProjectStatistics> ProjectStatistics { get; set; }
    public DbSet<BranchHistory> BranchHistories { get; set; }
    public DbSet<SbomPackage> SbomPackages { get; set; }
    public DbSet<Reference> References { get; set; }
    public DbSet<PackageDependency> PackageDependencies => Set<PackageDependency>();
    public DbSet<Vulnerability> Vulnerabilities { get; set; }
    public DbSet<SbomPackageVulnerability> SbomPackageVulnerabilities =>
        Set<SbomPackageVulnerability>();
    public DbSet<ProjectBranch> ProjectBranches { get; set; }

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

        modelBuilder
            .Entity<Vulnerability>()
            .HasMany(v => v.CWES)
            .WithMany(c => c.Vulnerabilities)
            .UsingEntity(j => j.ToTable("VulnerabilityCWEs"));

        modelBuilder.Entity<Vulnerability>().HasKey(x => x.Id);
        modelBuilder.Entity<Vulnerability>().HasIndex(x => x.Id).IsUnique();

        modelBuilder
            .Entity<ProjectBranch>()
            .HasOne(pb => pb.Sbom)
            .WithOne(s => s.ProjectBranch)
            .HasForeignKey<Sbom>(s => s.ProjectBranchId)
            .IsRequired(false);

        modelBuilder
            .Entity<BranchHistory>()
            .HasOne(bh => bh.Sbom)
            .WithOne(s => s.BranchHistory)
            .HasForeignKey<Sbom>(s => s.BranchHistoryId)
            .IsRequired(false);

        modelBuilder
            .Entity<Sbom>()
            .HasIndex(s => new { s.ProjectBranchId, s.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_Sboms_ProjectBranchId_CreatedAt");

        modelBuilder
            .Entity<Sbom>()
            .HasIndex(s => new { s.BranchHistoryId, s.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_Sboms_BranchHistoryId_CreatedAt");

        modelBuilder
            .Entity<ProjectBranch>()
            .HasIndex(pb => new { pb.ProjectId, pb.Name })
            .HasDatabaseName("IX_ProjectBranches_ProjectId_Name");

        modelBuilder
            .Entity<BranchHistory>()
            .HasIndex(bh => new { bh.ProjectBranchId, bh.ProcessStatus })
            .HasDatabaseName("IX_BranchHistories_ProjectBranchId_ProcessStatus");

        modelBuilder
            .Entity<BranchHistory>()
            .HasIndex(bh => new { bh.ProjectBranchId, bh.ProcessState })
            .HasDatabaseName("IX_BranchHistories_ProjectBranchId_ProcessState");

        modelBuilder.Entity<Sbom>().HasIndex(s => s.CommitSha).HasDatabaseName("IX_Sbom_CommitSha");

        modelBuilder
            .Entity<SbomPackage>()
            .HasIndex(sp => new
            {
                sp.Ecosystem,
                sp.Name,
                sp.Version,
            })
            .HasDatabaseName("IX_SbomPackage_Ecosystem_Name_Version");
    }
}
