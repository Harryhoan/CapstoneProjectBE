using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFAQRemoveGoalUpdateUserAndProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Goals",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "PaymentAccount",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Story",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "Goals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "Goals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDatetime",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDatetime",
                table: "Goals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goals",
                table: "Goals",
                columns: new[] { "ProjectId", "Question" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Goals",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "PaymentAccount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Story",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Answer",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "CreatedDatetime",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "UpdatedDatetime",
                table: "Goals");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Goals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Goals",
                table: "Goals",
                columns: new[] { "ProjectId", "Amount" });
        }
    }
}
