using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class fixedSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DefaultToCostReported",
                table: "Parts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Discriminator", "Name" },
                values: new object[,]
                {
                    { 1, "A new Part", "Condition", "New" },
                    { 18, "Used only for System B04", "PartType", "System B04" },
                    { 28, "Used only for System A07", "PartType", "System A07" },
                    { 27, "Used only for System A06", "PartType", "System A06" },
                    { 26, "Used only for System A05", "PartType", "System A05" },
                    { 25, "Used only for System A04", "PartType", "System A04" },
                    { 24, "Used only for System A03", "PartType", "System A03" },
                    { 23, "Used only for System A02", "PartType", "System A02" },
                    { 22, "Used only for System A01", "PartType", "System A01" },
                    { 21, "Used only for System B07", "PartType", "System B07" },
                    { 20, "Used only for System B06", "PartType", "System B06" },
                    { 7, "", "Organization", "Raw Materials" },
                    { 8, "", "Organization", "Supplies" },
                    { 13, "", "PartType", "1x2-short" },
                    { 14, "", "PartType", "1x2-long" },
                    { 15, "Used only for System B01", "PartType", "System B01" },
                    { 16, "Used only for System B02", "PartType", "System B02" },
                    { 17, "Used only for System B03", "PartType", "System B03" },
                    { 30, "Used only for System C02", "PartType", "System C02" },
                    { 31, "Used only for System C03", "PartType", "System C03" },
                    { 29, "Used only for System C01", "PartType", "System C01" },
                    { 33, "Used only for System C05", "PartType", "System C05" },
                    { 2, "Part that has been used and returned to inventory", "Condition", "Used" },
                    { 3, "A part returned to inventory and needs cleaning. i.e. Satellites", "Condition", "Need Cleaning" },
                    { 4, "A part returned to inventory in need of repair/refurbish", "Condition", "Needs Repair" },
                    { 5, "A part in inventory that was repaired/refurbished", "Condition", "Refurbished" },
                    { 6, "A part's stock is depleted. No additional stock will be added or returned", "Condition", "Depleted" },
                    { 32, "Used only for System C04", "PartType", "System C04" },
                    { 12, "Used on C Systems", "Usage", "C Systems" },
                    { 11, "Used on B Systems", "Usage", "B Systems" },
                    { 19, "Used only for System B05", "PartType", "System B05" },
                    { 9, "Used on all Epi Systems", "Usage", "All Systems" },
                    { 34, "Used only for System C06", "PartType", "System C06" },
                    { 35, "Used only for System C07", "PartType", "System C07" },
                    { 39, "Used only for System C11", "PartType", "System C11" },
                    { 36, "Used only for System C08", "PartType", "System C08" },
                    { 37, "Used only for System C09", "PartType", "System C09" },
                    { 38, "Used only for System C10", "PartType", "System C10" },
                    { 10, "Used on A Systems", "Usage", "A Systems" }
                });

            migrationBuilder.InsertData(
                table: "Distributors",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 3, "SiC & TaC Coated Graphite parts", "Mersen" },
                    { 6, "", "Akzo Nobel" },
                    { 5, "All Quartz Parts", "Quality Quartz Engineering " },
                    { 4, "Original Aixtron Parts", "Aixtron" },
                    { 2, "Sapphire Parts", "Rayotek" },
                    { 1, "Boron Nitride Parts", "LSP Industrial Ceramics Inc." }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Description", "Discriminator", "Name" },
                values: new object[,]
                {
                    { 3, "", "Warehouse", "Epi Chase" },
                    { 1, "", "Warehouse", "Epi System Parts" },
                    { 2, "", "Warehouse", "Gas Bay" },
                    { 5, "", "Warehouse", "Back Warehouse" },
                    { 31, "Generic Consumer for cost reporting", "Consumer", "Epi Process" },
                    { 6, "Reactor B01", "Consumer", "System B01" },
                    { 7, "Reactor B02", "Consumer", "System B02" },
                    { 8, "Reactor B03", "Consumer", "System B03" },
                    { 9, "Reactor B04", "Consumer", "System B04" },
                    { 10, "Reactor B05", "Consumer", "System B05" },
                    { 11, "Reactor B06", "Consumer", "System B06" },
                    { 12, "Reactor B07", "Consumer", "System B07" },
                    { 13, "Reactor A01", "Consumer", "System A01" },
                    { 14, "Reactor A02", "Consumer", "System A02" },
                    { 15, "Reactor A03", "Consumer", "System A03" },
                    { 16, "Reactor A04", "Consumer", "System A04" },
                    { 17, "Reactor A05", "Consumer", "System A05" },
                    { 18, "Reactor A06", "Consumer", "System A06" },
                    { 19, "Reactor A07", "Consumer", "System A07" },
                    { 20, "Reactor C01", "Consumer", "System C01" },
                    { 21, "Reactor C02", "Consumer", "System C02" },
                    { 22, "Reactor C03", "Consumer", "System C03" },
                    { 23, "Reactor C04", "Consumer", "System C04" },
                    { 24, "Reactor C05", "Consumer", "System C05" },
                    { 25, "Reactor C06", "Consumer", "System C06" },
                    { 26, "Reactor C07", "Consumer", "System C07" },
                    { 27, "Reactor C08", "Consumer", "System C08" },
                    { 28, "Reactor C09", "Consumer", "System C09" },
                    { 29, "Reactor C10", "Consumer", "System C10" },
                    { 30, "Reactor C11", "Consumer", "System C11" },
                    { 4, "", "Warehouse", "Process Lab" }
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "Comments", "Description", "Name" },
                values: new object[] { 1, null, "Mersen deals with all SiC coated & TaC coated graphite parts.", "Mersen" });
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
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 39);

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

            migrationBuilder.DropColumn(
                name: "DefaultToCostReported",
                table: "Parts");
        }
    }
}
