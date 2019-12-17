using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class CostCalcMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Prices");

            migrationBuilder.AddColumn<double>(
                name: "UnitCost",
                table: "Prices",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CostCalcMethod",
                table: "PartInstances",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "CostCalcMethod",
                table: "PartInstances");

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
