using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class partInstanceRework2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasuredWeight",
                table: "PartInstances");

            migrationBuilder.AddColumn<double>(
                name: "Measured",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Tare",
                table: "PartInstances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Measured",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Tare",
                table: "PartInstances");

            migrationBuilder.AddColumn<double>(
                name: "MeasuredWeight",
                table: "PartInstances",
                type: "float",
                nullable: true);
        }
    }
}
