using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageVulnerabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SbomPackages_Vulnerabilities_VulnerabilityId",
                table: "SbomPackages");

            migrationBuilder.DropIndex(
                name: "IX_SbomPackages_VulnerabilityId",
                table: "SbomPackages");

            migrationBuilder.DropColumn(
                name: "VulnerabilityId",
                table: "SbomPackages");

            migrationBuilder.CreateTable(
                name: "PackageVulnerabilities",
                columns: table => new
                {
                    VulnerabilityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SbomPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVulnerabilities", x => new { x.SbomPackageId, x.VulnerabilityId });
                    table.ForeignKey(
                        name: "FK_PackageVulnerabilities_SbomPackages_SbomPackageId",
                        column: x => x.SbomPackageId,
                        principalTable: "SbomPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageVulnerabilities_Vulnerabilities_VulnerabilityId",
                        column: x => x.VulnerabilityId,
                        principalTable: "Vulnerabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SbomPackageVulnerability",
                columns: table => new
                {
                    AffectedPackagesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VulnerabilitiesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SbomPackageVulnerability", x => new { x.AffectedPackagesId, x.VulnerabilitiesId });
                    table.ForeignKey(
                        name: "FK_SbomPackageVulnerability_SbomPackages_AffectedPackagesId",
                        column: x => x.AffectedPackagesId,
                        principalTable: "SbomPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SbomPackageVulnerability_Vulnerabilities_VulnerabilitiesId",
                        column: x => x.VulnerabilitiesId,
                        principalTable: "Vulnerabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageVulnerabilities_VulnerabilityId",
                table: "PackageVulnerabilities",
                column: "VulnerabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackageVulnerability_VulnerabilitiesId",
                table: "SbomPackageVulnerability",
                column: "VulnerabilitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageVulnerabilities");

            migrationBuilder.DropTable(
                name: "SbomPackageVulnerability");

            migrationBuilder.AddColumn<string>(
                name: "VulnerabilityId",
                table: "SbomPackages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackages_VulnerabilityId",
                table: "SbomPackages",
                column: "VulnerabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SbomPackages_Vulnerabilities_VulnerabilityId",
                table: "SbomPackages",
                column: "VulnerabilityId",
                principalTable: "Vulnerabilities",
                principalColumn: "Id");
        }
    }
}
