using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class SetToNoACtion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SbomPackageVulnerabilities_SbomPackages_SbomPackageId",
                table: "SbomPackageVulnerabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_SbomPackageVulnerabilities_Vulnerabilities_VulnerabilityId",
                table: "SbomPackageVulnerabilities");

            migrationBuilder.AddForeignKey(
                name: "FK_SbomPackageVulnerabilities_SbomPackages_SbomPackageId",
                table: "SbomPackageVulnerabilities",
                column: "SbomPackageId",
                principalTable: "SbomPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SbomPackageVulnerabilities_Vulnerabilities_VulnerabilityId",
                table: "SbomPackageVulnerabilities",
                column: "VulnerabilityId",
                principalTable: "Vulnerabilities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SbomPackageVulnerabilities_SbomPackages_SbomPackageId",
                table: "SbomPackageVulnerabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_SbomPackageVulnerabilities_Vulnerabilities_VulnerabilityId",
                table: "SbomPackageVulnerabilities");

            migrationBuilder.AddForeignKey(
                name: "FK_SbomPackageVulnerabilities_SbomPackages_SbomPackageId",
                table: "SbomPackageVulnerabilities",
                column: "SbomPackageId",
                principalTable: "SbomPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SbomPackageVulnerabilities_Vulnerabilities_VulnerabilityId",
                table: "SbomPackageVulnerabilities",
                column: "VulnerabilityId",
                principalTable: "Vulnerabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
