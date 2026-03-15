using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepVis.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectStatisticsLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EcoSystems",
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

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatistics_ProjectId",
                table: "ProjectStatistics",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStatistics_Projects_ProjectId",
                table: "ProjectStatistics",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStatistics_Projects_ProjectId",
                table: "ProjectStatistics");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStatistics_ProjectId",
                table: "ProjectStatistics");

            migrationBuilder.DropColumn(
                name: "EcoSystems",
                table: "ProjectStatistics");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ProjectStatistics");
        }
    }
}
