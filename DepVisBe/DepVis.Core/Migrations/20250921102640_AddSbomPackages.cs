using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSbomPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sbom_Projects_ProjectId",
                table: "Sbom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sbom",
                table: "Sbom");

            migrationBuilder.RenameTable(
                name: "Sbom",
                newName: "Sboms");

            migrationBuilder.RenameIndex(
                name: "IX_Sbom_ProjectId",
                table: "Sboms",
                newName: "IX_Sboms_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sboms",
                table: "Sboms",
                column: "Id");

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
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackages_SbomId",
                table: "SbomPackages",
                column: "SbomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sboms_Projects_ProjectId",
                table: "Sboms",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sboms_Projects_ProjectId",
                table: "Sboms");

            migrationBuilder.DropTable(
                name: "SbomPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sboms",
                table: "Sboms");

            migrationBuilder.RenameTable(
                name: "Sboms",
                newName: "Sbom");

            migrationBuilder.RenameIndex(
                name: "IX_Sboms_ProjectId",
                table: "Sbom",
                newName: "IX_Sbom_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sbom",
                table: "Sbom",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sbom_Projects_ProjectId",
                table: "Sbom",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
