using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizOpsAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeRevenueInvoiceOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Revenues_Invoices_InvoiceId",
                table: "Revenues");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "Revenues",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Revenues_Invoices_InvoiceId",
                table: "Revenues",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Revenues_Invoices_InvoiceId",
                table: "Revenues");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "Revenues",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Revenues_Invoices_InvoiceId",
                table: "Revenues",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
