using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizOpsAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterEmailIngestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailMessageId",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailThreadId",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceEmailSentAt",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePdfFileName",
                table: "Invoices",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailMessageId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EmailThreadId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceEmailSentAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoicePdfFileName",
                table: "Invoices");
        }
    }
}
