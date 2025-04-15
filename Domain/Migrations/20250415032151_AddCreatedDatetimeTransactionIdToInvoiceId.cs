using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDatetimeTransactionIdToInvoiceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "PledgeDetails",
                newName: "InvoiceId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDatetime",
                table: "PledgeDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDatetime",
                table: "PledgeDetails");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "PledgeDetails",
                newName: "TransactionId");
        }
    }
}
