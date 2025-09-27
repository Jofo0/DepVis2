using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class NewInitialMigration : Migration
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
                    ProcessStep = table.Column<int>(type: "int", nullable: false),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false),
                    ProjectLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sboms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sboms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sboms_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
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

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_ChildId",
                table: "PackageDependencies",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackages_SbomId",
                table: "SbomPackages",
                column: "SbomId");

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_ProjectId",
                table: "Sboms",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageDependencies");

            migrationBuilder.DropTable(
                name: "SbomPackages");

            migrationBuilder.DropTable(
                name: "Sboms");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
