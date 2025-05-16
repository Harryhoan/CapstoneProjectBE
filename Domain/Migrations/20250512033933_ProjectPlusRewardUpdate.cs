using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class ProjectPlusRewardUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDatetime",
                table: "Projects",
                newName: "UpdatedDatetime");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Rewards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDatetime",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "CreatedDatetime",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "UpdatedDatetime",
                table: "Projects",
                newName: "UpdateDatetime");
        }
    }
}
