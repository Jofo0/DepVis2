using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class ReworkProjectStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStatistics_Projects_ProjectId",
                table: "ProjectStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_Sboms_Projects_ProjectId",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStatistics_ProjectId",
                table: "ProjectStatistics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectBranches",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "Branch",
                table: "Sboms");

            migrationBuilder.DropColumn(
                name: "Branch",
                table: "ProjectStatistics");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectStatistics");

            migrationBuilder.DropColumn(
                name: "ProcessStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProcessStep",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Sboms",
                newName: "ProjectBranchId");

            migrationBuilder.RenameIndex(
                name: "IX_Sboms_ProjectId",
                table: "Sboms",
                newName: "IX_Sboms_ProjectBranchId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectBranches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProjectBranches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "PackageCount",
                table: "ProjectBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessStatus",
                table: "ProjectBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessStep",
                table: "ProjectBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VulnerabilityCount",
                table: "ProjectBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectBranches",
                table: "ProjectBranches",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBranches_ProjectId",
                table: "ProjectBranches",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                table: "Sboms",
                column: "ProjectBranchId",
                principalTable: "ProjectBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sboms_ProjectBranches_ProjectBranchId",
                table: "Sboms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectBranches",
                table: "ProjectBranches");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBranches_ProjectId",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "PackageCount",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "ProcessStatus",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "ProcessStep",
                table: "ProjectBranches");

            migrationBuilder.DropColumn(
                name: "VulnerabilityCount",
                table: "ProjectBranches");

            migrationBuilder.RenameColumn(
                name: "ProjectBranchId",
                table: "Sboms",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Sboms_ProjectBranchId",
                table: "Sboms",
                newName: "IX_Sboms_ProjectId");

            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "Sboms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "ProjectStatistics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "ProjectStatistics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ProcessStatus",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessStep",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectBranches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectBranches",
                table: "ProjectBranches",
                columns: new[] { "ProjectId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatistics_ProjectId",
                table: "ProjectStatistics",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStatistics_Projects_ProjectId",
                table: "ProjectStatistics",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sboms_Projects_ProjectId",
                table: "Sboms",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
