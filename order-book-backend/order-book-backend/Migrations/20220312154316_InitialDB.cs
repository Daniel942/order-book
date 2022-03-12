using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace order_book_backend.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderBook",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBook", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ask",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderBookID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ask", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ask_OrderBook_OrderBookID",
                        column: x => x.OrderBookID,
                        principalTable: "OrderBook",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Bid",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderBookID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bid", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bid_OrderBook_OrderBookID",
                        column: x => x.OrderBookID,
                        principalTable: "OrderBook",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ask_OrderBookID",
                table: "Ask",
                column: "OrderBookID");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_OrderBookID",
                table: "Bid",
                column: "OrderBookID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ask");

            migrationBuilder.DropTable(
                name: "Bid");

            migrationBuilder.DropTable(
                name: "OrderBook");
        }
    }
}
