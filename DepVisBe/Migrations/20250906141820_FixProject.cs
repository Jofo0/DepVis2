using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVisBe.Migrations
{
    /// <inheritdoc />
    public partial class FixProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FolderPath",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GitHubLink",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FolderPath",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "GitHubLink",
                table: "Projects");
        }
    }
}
