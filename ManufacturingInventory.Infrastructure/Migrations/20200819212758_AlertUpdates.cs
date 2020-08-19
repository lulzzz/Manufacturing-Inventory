using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class AlertUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Categories_StockId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_StockId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "AlertTime",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "PartId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Alerts");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "UserAlerts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AlertId",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AlertId",
                table: "Categories",
                column: "AlertId",
                unique: true,
                filter: "[AlertId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Alerts_AlertId",
                table: "Categories",
                column: "AlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Alerts_AlertId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_AlertId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "UserAlerts");

            migrationBuilder.DropColumn(
                name: "AlertId",
                table: "Categories");

            migrationBuilder.AddColumn<DateTime>(
                name: "AlertTime",
                table: "Alerts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Alerts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PartId",
                table: "Alerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Alerts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_StockId",
                table: "Alerts",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Categories_StockId",
                table: "Alerts",
                column: "StockId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
