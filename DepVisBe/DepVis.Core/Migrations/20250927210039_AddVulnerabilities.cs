using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddVulnerabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VulnerabilityId",
                table: "SbomPackages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Vulnerabilities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerabilities", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SbomPackages_Vulnerabilities_VulnerabilityId",
                table: "SbomPackages");

            migrationBuilder.DropTable(
                name: "Vulnerabilities");

            migrationBuilder.DropIndex(
                name: "IX_SbomPackages_VulnerabilityId",
                table: "SbomPackages");

            migrationBuilder.DropColumn(
                name: "VulnerabilityId",
                table: "SbomPackages");
        }
    }
}
