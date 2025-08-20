using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizOpsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRevenueSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Revenues",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Revenues");
        }
    }
}
