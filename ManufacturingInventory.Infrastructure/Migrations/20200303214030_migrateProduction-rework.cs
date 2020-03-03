using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class migrateProductionrework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Discriminator", "IsDefault", "Name" },
                values: new object[,]
                {
                    { 1, "A new Part", "Condition", true, "New" },
                    { 7, "", "Organization", false, "Raw Materials" },
                    { 8, "", "Organization", false, "Supplies" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Discriminator", "IsDefault", "Name", "MinQuantity", "Quantity", "SafeQuantity" },
                values: new object[] { 14, "Individual Stock", "StockType", true, "Individual", 0, 0, 0 });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Discriminator", "IsDefault", "Name" },
                values: new object[,]
                {
                    { 10, "General Growth Usage", "Usage", true, "Growth" },
                    { 11, "Used on A Systems", "Usage", false, "A Systems" },
                    { 12, "Used on B Systems", "Usage", false, "B Systems" },
                    { 13, "Used on C Systems", "Usage", false, "C Systems" },
                    { 9, "Used on all Epi Systems", "Usage", false, "All Systems" },
                    { 6, "A part's stock is depleted. No additional stock will be added or returned", "Condition", false, "Depleted" },
                    { 5, "A part in inventory that was repaired/refurbished", "Condition", false, "Refurbished" },
                    { 4, "A part returned to inventory in need of repair/refurbish", "Condition", false, "Needs Repair" },
                    { 3, "A part returned to inventory and needs cleaning. i.e. Satellites", "Condition", false, "Need Cleaning" },
                    { 2, "Part that has been used and returned to inventory", "Condition", false, "Used" }
                });

            migrationBuilder.InsertData(
                table: "Distributors",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 6, "", "Akzo Nobel" },
                    { 1, "Boron Nitride Parts", "LSP Industrial Ceramics Inc." },
                    { 5, "All Quartz Parts", "Quality Quartz Engineering " },
                    { 4, "Original Aixtron Parts", "Aixtron" },
                    { 3, "SiC & TaC Coated Graphite parts", "Mersen" },
                    { 2, "Sapphire Parts", "Rayotek" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Description", "Discriminator", "IsDefualt", "Name" },
                values: new object[,]
                {
                    { 30, "Reactor C11", "Consumer", false, "System C11" },
                    { 31, "Generic Consumer for cost reporting", "Consumer", false, "Epi Process" },
                    { 2, "", "Warehouse", false, "Gas Bay" },
                    { 29, "Reactor C10", "Consumer", false, "System C10" },
                    { 3, "", "Warehouse", false, "Epi Chase" },
                    { 1, "", "Warehouse", false, "Epi System Parts" },
                    { 28, "Reactor C09", "Consumer", false, "System C09" },
                    { 27, "Reactor C08", "Consumer", false, "System C08" },
                    { 26, "Reactor C07", "Consumer", false, "System C07" },
                    { 6, "Reactor B01", "Consumer", false, "System B01" },
                    { 7, "Reactor B02", "Consumer", false, "System B02" },
                    { 8, "Reactor B03", "Consumer", false, "System B03" },
                    { 9, "Reactor B04", "Consumer", false, "System B04" },
                    { 10, "Reactor B05", "Consumer", false, "System B05" },
                    { 11, "Reactor B06", "Consumer", false, "System B06" },
                    { 12, "Reactor B07", "Consumer", false, "System B07" },
                    { 13, "Reactor A01", "Consumer", false, "System A01" },
                    { 14, "Reactor A02", "Consumer", false, "System A02" },
                    { 4, "", "Warehouse", false, "Process Lab" },
                    { 15, "Reactor A03", "Consumer", false, "System A03" },
                    { 5, "", "Warehouse", false, "Back Warehouse" },
                    { 17, "Reactor A05", "Consumer", false, "System A05" },
                    { 18, "Reactor A06", "Consumer", false, "System A06" },
                    { 19, "Reactor A07", "Consumer", false, "System A07" },
                    { 20, "Reactor C01", "Consumer", false, "System C01" },
                    { 21, "Reactor C02", "Consumer", false, "System C02" },
                    { 22, "Reactor C03", "Consumer", false, "System C03" },
                    { 23, "Reactor C04", "Consumer", false, "System C04" },
                    { 24, "Reactor C05", "Consumer", false, "System C05" },
                    { 25, "Reactor C06", "Consumer", false, "System C06" },
                    { 16, "Reactor A04", "Consumer", false, "System A04" }
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "Comments", "Description", "Name" },
                values: new object[] { 1, null, "Mersen deals with all SiC coated & TaC coated graphite parts.", "Mersen" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 2, "Inventory View Only", "InventoryUserAccount" },
                    { 3, "Full Inventory Privileges", "InventoryUserFullAccount" },
                    { 4, "Inventory Check In/Check Out/Create", "InventoryUserLimitedAccount" },
                    { 1, "Full Inventory Privileges and User Control", "InventoryAdminAccount" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EncryptedPassword", "Extension", "FirstName", "IV", "Key", "LastName", "PermissionId", "StorePassword", "UserName" },
                values: new object[] { 1, null, null, null, "Andrew", null, null, "Elmendorf", 1, false, "AElmendo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 1);

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
