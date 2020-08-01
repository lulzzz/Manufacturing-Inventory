using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class MonthlySummaryrev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PartMonthlySummary",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "MonthlySummaries",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PartMonthlySummary");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "MonthlySummaries",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldRowVersion: true,
                oldNullable: true);
        }
    }
}
