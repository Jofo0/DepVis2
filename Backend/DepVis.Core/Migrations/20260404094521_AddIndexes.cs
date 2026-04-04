using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sboms_BranchHistoryId",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_Sboms_ProjectBranchId",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBranches_ProjectId",
                table: "ProjectBranches");

            migrationBuilder.DropIndex(
                name: "IX_BranchHistories_ProjectBranchId",
                table: "BranchHistories");

            migrationBuilder.AlterColumn<string>(
                name: "CommitSha",
                table: "Sboms",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "SbomPackages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SbomPackages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Ecosystem",
                table: "SbomPackages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectBranches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Sbom_CommitSha",
                table: "Sboms",
                column: "CommitSha");

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_BranchHistoryId_CreatedAt",
                table: "Sboms",
                columns: new[] { "BranchHistoryId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_ProjectBranchId_CreatedAt",
                table: "Sboms",
                columns: new[] { "ProjectBranchId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_SbomPackage_Ecosystem_Name_Version",
                table: "SbomPackages",
                columns: new[] { "Ecosystem", "Name", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBranches_ProjectId_Name",
                table: "ProjectBranches",
                columns: new[] { "ProjectId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchHistories_ProjectBranchId_ProcessState",
                table: "BranchHistories",
                columns: new[] { "ProjectBranchId", "ProcessState" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchHistories_ProjectBranchId_ProcessStatus",
                table: "BranchHistories",
                columns: new[] { "ProjectBranchId", "ProcessStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sbom_CommitSha",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_Sboms_BranchHistoryId_CreatedAt",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_Sboms_ProjectBranchId_CreatedAt",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_SbomPackage_Ecosystem_Name_Version",
                table: "SbomPackages");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBranches_ProjectId_Name",
                table: "ProjectBranches");

            migrationBuilder.DropIndex(
                name: "IX_BranchHistories_ProjectBranchId_ProcessState",
                table: "BranchHistories");

            migrationBuilder.DropIndex(
                name: "IX_BranchHistories_ProjectBranchId_ProcessStatus",
                table: "BranchHistories");

            migrationBuilder.AlterColumn<string>(
                name: "CommitSha",
                table: "Sboms",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "SbomPackages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SbomPackages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Ecosystem",
                table: "SbomPackages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProjectBranches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_BranchHistoryId",
                table: "Sboms",
                column: "BranchHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_ProjectBranchId",
                table: "Sboms",
                column: "ProjectBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBranches_ProjectId",
                table: "ProjectBranches",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchHistories_ProjectBranchId",
                table: "BranchHistories",
                column: "ProjectBranchId");
        }
    }
}
