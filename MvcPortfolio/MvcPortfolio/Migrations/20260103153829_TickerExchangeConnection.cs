using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class TickerExchangeConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ticker_ExchangeString",
                table: "Ticker",
                column: "ExchangeString");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticker_Exchange_ExchangeString",
                table: "Ticker",
                column: "ExchangeString",
                principalTable: "Exchange",
                principalColumn: "ExchangeString",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticker_Exchange_ExchangeString",
                table: "Ticker");

            migrationBuilder.DropIndex(
                name: "IX_Ticker_ExchangeString",
                table: "Ticker");
        }
    }
}
