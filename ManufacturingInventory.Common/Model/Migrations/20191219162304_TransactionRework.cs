using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class TransactionRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Locations_WarehouseId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Locations_ConsumerId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_WarehouseId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ConsumerId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ConsumerId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReturningTransactionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ParameterValue",
                table: "Transactions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LocationId",
                table: "Transactions",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LocationId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ParameterValue",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsumerId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturningTransactionId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WarehouseId",
                table: "Transactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ConsumerId",
                table: "Transactions",
                column: "ConsumerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Locations_WarehouseId",
                table: "Transactions",
                column: "WarehouseId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Locations_ConsumerId",
                table: "Transactions",
                column: "ConsumerId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
