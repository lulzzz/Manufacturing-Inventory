using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class PartInstanceRework3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateInstalled",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "DateRemoved",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Measured",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Tare",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PartInstances");

            migrationBuilder.AddColumn<int>(
                name: "BubblerParameterId",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBubbler",
                table: "PartInstances",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BubblerParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NetWeight = table.Column<double>(nullable: false),
                    Tare = table.Column<double>(nullable: false),
                    GrossWeight = table.Column<double>(nullable: false),
                    Measured = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    DateInstalled = table.Column<DateTime>(nullable: true),
                    DateRemoved = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BubblerParameters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_BubblerParameterId",
                table: "PartInstances",
                column: "BubblerParameterId",
                unique: true,
                filter: "[BubblerParameterId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_BubblerParameters_BubblerParameterId",
                table: "PartInstances",
                column: "BubblerParameterId",
                principalTable: "BubblerParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_BubblerParameters_BubblerParameterId",
                table: "PartInstances");

            migrationBuilder.DropTable(
                name: "BubblerParameters");

            migrationBuilder.DropIndex(
                name: "IX_PartInstances_BubblerParameterId",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "BubblerParameterId",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "IsBubbler",
                table: "PartInstances");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInstalled",
                table: "PartInstances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRemoved",
                table: "PartInstances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GrossWeight",
                table: "PartInstances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Measured",
                table: "PartInstances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NetWeight",
                table: "PartInstances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Tare",
                table: "PartInstances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "PartInstances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "PartInstances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
