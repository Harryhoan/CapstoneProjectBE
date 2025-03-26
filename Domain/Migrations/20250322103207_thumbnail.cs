using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class thumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinmumAmount",
                table: "Projects",
                newName: "MinimumAmount");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Projects",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "MinimumAmount",
                table: "Projects",
                newName: "MinmumAmount");
        }
    }
}
