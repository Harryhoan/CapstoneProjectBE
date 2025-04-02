using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class new2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Posts\" ALTER COLUMN \"Status\" TYPE integer USING \"Status\"::integer;");
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.Sql("ALTER TABLE \"PledgeDetails\" ALTER COLUMN \"Status\" TYPE integer USING \"Status\"::integer;");
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PledgeDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.Sql("ALTER TABLE \"Projects\" ALTER COLUMN \"Status\" TYPE integer USING \"Status\"::integer;");
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Projects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PledgeDetails",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Projects",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
