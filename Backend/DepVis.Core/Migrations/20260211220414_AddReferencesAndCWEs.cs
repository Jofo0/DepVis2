using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddReferencesAndCWEs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CWEs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CweId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CWEs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "References",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VulnerabilityId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_References", x => x.Id);
                    table.ForeignKey(
                        name: "FK_References_Vulnerabilities_VulnerabilityId",
                        column: x => x.VulnerabilityId,
                        principalTable: "Vulnerabilities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VulnerabilityCWEs",
                columns: table => new
                {
                    CWESId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VulnerabilitiesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VulnerabilityCWEs", x => new { x.CWESId, x.VulnerabilitiesId });
                    table.ForeignKey(
                        name: "FK_VulnerabilityCWEs_CWEs_CWESId",
                        column: x => x.CWESId,
                        principalTable: "CWEs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VulnerabilityCWEs_Vulnerabilities_VulnerabilitiesId",
                        column: x => x.VulnerabilitiesId,
                        principalTable: "Vulnerabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_References_VulnerabilityId",
                table: "References",
                column: "VulnerabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_VulnerabilityCWEs_VulnerabilitiesId",
                table: "VulnerabilityCWEs",
                column: "VulnerabilitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "References");

            migrationBuilder.DropTable(
                name: "VulnerabilityCWEs");

            migrationBuilder.DropTable(
                name: "CWEs");
        }
    }
}
