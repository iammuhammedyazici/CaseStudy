using System;
using ECommerce.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Stock.Infrastructure.Migrations;

[DbContext(typeof(StockDbContext))]
[Migration("20250102000000_UpdateStockSchema")]
public partial class UpdateStockSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "VariantId",
            table: "StockItems",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "ReservedQuantity",
            table: "StockItems",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "StockItems",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "StockItems",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "StockItems",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "StockItems",
            type: "bytea",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "MinimumQuantity",
            table: "StockItems",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<bool>(
            name: "IsActive",
            table: "StockItems",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<int>(
            name: "VariantId",
            table: "StockReservations",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.Sql("UPDATE \"StockItems\" SET \"VariantId\" = \"ProductId\" WHERE \"VariantId\" = 0;");
        migrationBuilder.Sql("UPDATE \"StockItems\" SET \"CreatedAt\" = NOW() WHERE \"CreatedAt\" IS NULL;");
        migrationBuilder.Sql("UPDATE \"StockItems\" SET \"RowVersion\" = decode('', 'hex') WHERE \"RowVersion\" IS NULL;");
        migrationBuilder.Sql("UPDATE \"StockReservations\" SET \"VariantId\" = \"ProductId\" WHERE \"VariantId\" = 0;");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            table: "StockItems",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone",
            oldNullable: true);

        migrationBuilder.AlterColumn<byte[]>(
            name: "RowVersion",
            table: "StockItems",
            type: "bytea",
            rowVersion: true,
            nullable: false,
            oldClrType: typeof(byte[]),
            oldType: "bytea",
            oldNullable: true);

        migrationBuilder.DropPrimaryKey(
            name: "PK_StockItems",
            table: "StockItems");

        migrationBuilder.AddPrimaryKey(
            name: "PK_StockItems",
            table: "StockItems",
            column: "VariantId");

        migrationBuilder.CreateIndex(
            name: "IX_StockItems_ProductId",
            table: "StockItems",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_StockReservations_OrderId",
            table: "StockReservations",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_StockReservations_VariantId",
            table: "StockReservations",
            column: "VariantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_StockItems_ProductId",
            table: "StockItems");

        migrationBuilder.DropIndex(
            name: "IX_StockReservations_OrderId",
            table: "StockReservations");

        migrationBuilder.DropIndex(
            name: "IX_StockReservations_VariantId",
            table: "StockReservations");

        migrationBuilder.DropPrimaryKey(
            name: "PK_StockItems",
            table: "StockItems");

        migrationBuilder.AddPrimaryKey(
            name: "PK_StockItems",
            table: "StockItems",
            column: "ProductId");

        migrationBuilder.DropColumn(
            name: "VariantId",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "ReservedQuantity",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "MinimumQuantity",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "IsActive",
            table: "StockItems");

        migrationBuilder.DropColumn(
            name: "VariantId",
            table: "StockReservations");
    }
}
