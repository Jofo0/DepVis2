using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class sboms1t1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sboms_BranchHistoryId",
                table: "Sboms",
                column: "BranchHistoryId",
                unique: true,
                filter: "[BranchHistoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sboms_ProjectBranchId",
                table: "Sboms",
                column: "ProjectBranchId",
                unique: true,
                filter: "[ProjectBranchId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sboms_BranchHistoryId",
                table: "Sboms");

            migrationBuilder.DropIndex(
                name: "IX_Sboms_ProjectBranchId",
                table: "Sboms");
        }
    }
}
