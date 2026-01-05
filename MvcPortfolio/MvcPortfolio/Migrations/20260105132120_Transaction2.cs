using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class Transaction2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TickerString = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Buy = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Ticker_TickerString",
                        column: x => x.TickerString,
                        principalTable: "Ticker",
                        principalColumn: "TickerString",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TickerString",
                table: "Transaction",
                column: "TickerString");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction");
        }
    }
}
