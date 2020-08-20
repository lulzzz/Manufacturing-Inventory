using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class AlertUpdate : Migration
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
                name: "IndividualAlertId",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CombinedAlertId",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "alert_type",
                table: "Alerts",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_IndividualAlertId",
                table: "PartInstances",
                column: "IndividualAlertId",
                unique: true,
                filter: "[IndividualAlertId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CombinedAlertId",
                table: "Categories",
                column: "CombinedAlertId",
                unique: true,
                filter: "[CombinedAlertId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Alerts_CombinedAlertId",
                table: "Categories",
                column: "CombinedAlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Alerts_IndividualAlertId",
                table: "PartInstances",
                column: "IndividualAlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Alerts_CombinedAlertId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Alerts_IndividualAlertId",
                table: "PartInstances");

            migrationBuilder.DropIndex(
                name: "IX_PartInstances_IndividualAlertId",
                table: "PartInstances");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CombinedAlertId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "UserAlerts");

            migrationBuilder.DropColumn(
                name: "IndividualAlertId",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "CombinedAlertId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "alert_type",
                table: "Alerts");

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
