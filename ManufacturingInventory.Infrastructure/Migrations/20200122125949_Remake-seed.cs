using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class Remakeseed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Full Inventory Privileges and User Control", "InventoryAdminAccount" },
                    { 2, "Inventory View Only", "InventoryUserAccount" },
                    { 3, "Full Inventory Privileges", "InventoryUserFullAccount" },
                    { 4, "Inventory Check In/Check Out/Create", "InventoryUserLimitedAccount" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EncryptedPassword", "Extension", "FirstName", "IV", "Key", "LastName", "PermissionId", "StorePassword", "UserName" },
                values: new object[] { 1, null, null, null, "Andrew", null, null, "Elmendorf", 1, false, "AElmendo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
