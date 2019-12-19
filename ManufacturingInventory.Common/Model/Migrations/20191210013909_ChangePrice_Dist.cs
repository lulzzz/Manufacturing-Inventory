using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class ChangePrice_Dist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Price_DistributorId",
                table: "Price");

            migrationBuilder.AddColumn<bool>(
                name: "CostReported",
                table: "PartInstances",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Price_DistributorId",
                table: "Price",
                column: "DistributorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Price_DistributorId",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "CostReported",
                table: "PartInstances");

            migrationBuilder.CreateIndex(
                name: "IX_Price_DistributorId",
                table: "Price",
                column: "DistributorId",
                unique: true);
        }
    }
}
