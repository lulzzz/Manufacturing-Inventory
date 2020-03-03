using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class DateRemoveDateInstall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateInstalled",
                table: "BubblerParameters");

            migrationBuilder.DropColumn(
                name: "DateRemoved",
                table: "BubblerParameters");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInstalled",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRemoved",
                table: "PartInstances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateInstalled",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "DateRemoved",
                table: "PartInstances");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInstalled",
                table: "BubblerParameters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRemoved",
                table: "BubblerParameters",
                type: "datetime2",
                nullable: true);
        }
    }
}
