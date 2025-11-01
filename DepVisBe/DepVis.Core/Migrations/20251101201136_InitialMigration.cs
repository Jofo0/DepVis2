using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectType = table.Column<int>(type: "int", nullable: false),
                    ProjectLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageCount = table.Column<int>(type: "int", nullable: false),
                    VulnerabilityCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vulnerabilities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessStep = table.Column<int>(type: "int", nullable: false),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false),
                    IsTag = table.Column<bool>(type: "bit", nullable: false),
                    PackageCount = table.Column<int>(type: "int", nullable: false),
                    VulnerabilityCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBranches_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sboms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommitMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommitSha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sboms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                        column: x => x.ProjectBranchId,
                        principalTable: "ProjectBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SbomPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SbomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ecosystem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BomRef = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SbomPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SbomPackages_Sboms_SbomId",
                        column: x => x.SbomId,
                        principalTable: "Sboms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageDependencies",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDependencies", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_PackageDependencies_SbomPackages_ChildId",
                        column: x => x.ChildId,
                        principalTable: "SbomPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackageDependencies_SbomPackages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SbomPackages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SbomPackageVulnerabilities",
                columns: table => new
                {
                    VulnerabilityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SbomPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SbomPackageVulnerabilities", x => new { x.SbomPackageId, x.VulnerabilityId });
                    table.ForeignKey(
                        name: "FK_SbomPackageVulnerabilities_SbomPackages_SbomPackageId",
                        column: x => x.SbomPackageId,
                        principalTable: "SbomPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SbomPackageVulnerabilities_Vulnerabilities_VulnerabilityId",
                        column: x => x.VulnerabilityId,
                        principalTable: "Vulnerabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_ChildId",
                table: "PackageDependencies",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBranches_ProjectId",
                table: "ProjectBranches",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackages_SbomId",
                table: "SbomPackages",
                column: "SbomId");

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackageVulnerabilities_VulnerabilityId",
                table: "SbomPackageVulnerabilities",
                column: "VulnerabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_ProjectBranchId",
                table: "Sboms",
                column: "ProjectBranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageDependencies");

            migrationBuilder.DropTable(
                name: "ProjectStatistics");

            migrationBuilder.DropTable(
                name: "SbomPackageVulnerabilities");

            migrationBuilder.DropTable(
                name: "SbomPackages");

            migrationBuilder.DropTable(
                name: "Vulnerabilities");

            migrationBuilder.DropTable(
                name: "Sboms");

            migrationBuilder.DropTable(
                name: "ProjectBranches");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
