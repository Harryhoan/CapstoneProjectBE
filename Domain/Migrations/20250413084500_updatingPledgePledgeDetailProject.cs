using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatingPledgePledgeDetailProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Pledges",
                newName: "TotalAmount");

            migrationBuilder.AddColumn<int>(
                name: "TransactionStatus",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "PledgeDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                table: "PledgeDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "PledgeDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PledgeDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                table: "PledgeDetails");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "PledgeDetails");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Pledges",
                newName: "Amount");
        }
    }
}
