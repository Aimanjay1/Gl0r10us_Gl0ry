using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizOpsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailMessageId",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromAddress",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "Receipts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sha256Hex",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "Receipts",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "EmailMessageId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "FromAddress",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Sha256Hex",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "Receipts");
        }
    }
}
