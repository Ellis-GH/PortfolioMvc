using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class Exchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchange",
                columns: table => new
                {
                    ExchangeString = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AVCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatioToOne = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchange", x => x.ExchangeString);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exchange");
        }
    }
}
