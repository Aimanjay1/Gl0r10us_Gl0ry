using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizOpsAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixReceiptSizeBytesCheckForPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Safe drop: won't error if the constraint doesn't exist
            migrationBuilder.Sql(
                "ALTER TABLE \"Receipts\" DROP CONSTRAINT IF EXISTS \"CK_Receipt_SizeBytes_NonNegative\";");

            // Add the Postgres-safe check constraint
            migrationBuilder.AddCheckConstraint(
                name: "CK_Receipt_SizeBytes_NonNegative",
                table: "Receipts",
                sql: "\"SizeBytes\" IS NULL OR \"SizeBytes\" >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Just drop it if present (no SQL Server bracket syntax)
            migrationBuilder.Sql(
                "ALTER TABLE \"Receipts\" DROP CONSTRAINT IF EXISTS \"CK_Receipt_SizeBytes_NonNegative\";");
        }
    }
}
