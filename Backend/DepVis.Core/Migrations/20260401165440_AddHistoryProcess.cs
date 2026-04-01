using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessState",
                table: "BranchHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessState",
                table: "BranchHistories");
        }
    }
}
