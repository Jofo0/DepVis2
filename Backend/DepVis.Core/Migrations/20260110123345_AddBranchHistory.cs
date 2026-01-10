using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                table: "Sboms");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectBranchId",
                table: "Sboms",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchHistoryId",
                table: "Sboms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HistoryProcessinStatus",
                table: "ProjectBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HistoryProcessingStep",
                table: "ProjectBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BranchHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectBranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageCount = table.Column<int>(type: "int", nullable: false),
                    VulnerabilityCount = table.Column<int>(type: "int", nullable: false),
                    CommitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommitMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommitSha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchHistories_ProjectBranches_ProjectBranchId",
                        column: x => x.ProjectBranchId,
                        principalTable: "ProjectBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_BranchHistoryId",
                table: "Sboms",
                column: "BranchHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchHistories_ProjectBranchId",
                table: "BranchHistories",
                column: "ProjectBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sboms_BranchHistories_BranchHistoryId",
                table: "Sboms",
                column: "BranchHistoryId",
                principalTable: "BranchHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                table: "Sboms",
                column: "ProjectBranchId",
                principalTable: "ProjectBranches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sboms_BranchHistories_BranchHistoryId",
                table: "Sboms");

            migrationBuilder.DropForeignKey(
                name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                table: "Sboms");

            migrationBuilder.DropTable(
                name: "BranchHistories");

            migrationBuilder.DropIndex(
                name: "IX_Sboms_BranchHistoryId",
                table: "Sboms");

            migrationBuilder.DropColumn(
                name: "BranchHistoryId",
                table: "Sboms");

            migrationBuilder.DropColumn(
                name: "HistoryProcessinStatus",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "HistoryProcessingStep",
                table: "ProjectBranches");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectBranchId",
                table: "Sboms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                table: "Sboms",
                column: "ProjectBranchId",
                principalTable: "ProjectBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
