using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class Refractor2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VaildFrom",
                table: "Prices");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Prices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Prices");

            migrationBuilder.AddColumn<DateTime>(
                name: "VaildFrom",
                table: "Prices",
                type: "datetime2",
                nullable: true);
        }
    }
}
