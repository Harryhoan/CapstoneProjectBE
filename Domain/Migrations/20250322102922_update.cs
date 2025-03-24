using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonitorId",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_MonitorId",
                table: "Projects",
                column: "MonitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_MonitorId",
                table: "Projects",
                column: "MonitorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_MonitorId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_MonitorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MonitorId",
                table: "Projects");
        }
    }
}
