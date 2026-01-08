using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class ratiotooneMoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatioToOne",
                table: "Exchange");

            migrationBuilder.AddColumn<decimal>(
                name: "RatioToOne",
                table: "Ticker",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatioToOne",
                table: "Ticker");

            migrationBuilder.AddColumn<decimal>(
                name: "RatioToOne",
                table: "Exchange",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
