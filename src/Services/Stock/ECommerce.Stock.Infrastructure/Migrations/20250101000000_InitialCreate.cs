using System;
using ECommerce.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Stock.Infrastructure.Migrations;

[DbContext(typeof(StockDbContext))]
[Migration("20250101000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                Consumer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ReceivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InboxMessages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StockItems",
            columns: table => new
            {
                ProductId = table.Column<int>(type: "integer", nullable: false),
                AvailableQuantity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StockItems", x => x.ProductId);
            });

        migrationBuilder.CreateTable(
            name: "StockReservations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
                ReservedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StockReservations", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_InboxMessages_MessageId",
            table: "InboxMessages",
            column: "MessageId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InboxMessages");

        migrationBuilder.DropTable(
            name: "StockItems");

        migrationBuilder.DropTable(
            name: "StockReservations");
    }
}
