using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class CategoryRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_PartInstances_PartInstanceId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Categories_PartTypeId",
                table: "PartInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_UsageId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_UsageId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_PartInstances_PartTypeId",
                table: "PartInstances");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_PartInstanceId",
                table: "Alerts");

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

            migrationBuilder.DropColumn(
                name: "UsageId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "PartTypeId",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "PartInstanceId",
                table: "Alerts");

            migrationBuilder.AddColumn<int>(
                name: "StockTypeId",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageId",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinQuantity",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SafeQuantity",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartId",
                table: "Alerts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Alerts",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Discriminator", "Name" },
                values: new object[] { "StockType", "1x2-short" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Description", "Discriminator", "Name" },
                values: new object[] { "", "StockType", "1x2-long" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "General Growth Usage", "Growth" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Used on A Systems", "A Systems" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Used on B Systems", "B Systems" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "Discriminator", "Name" },
                values: new object[] { "Used on C Systems", "Usage", "C Systems" });

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_StockTypeId",
                table: "PartInstances",
                column: "StockTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_UsageId",
                table: "PartInstances",
                column: "UsageId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_StockId",
                table: "Alerts",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Categories_StockId",
                table: "Alerts",
                column: "StockId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Categories_StockTypeId",
                table: "PartInstances",
                column: "StockTypeId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Categories_UsageId",
                table: "PartInstances",
                column: "UsageId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Categories_StockId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Categories_StockTypeId",
                table: "PartInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Categories_UsageId",
                table: "PartInstances");

            migrationBuilder.DropIndex(
                name: "IX_PartInstances_StockTypeId",
                table: "PartInstances");

            migrationBuilder.DropIndex(
                name: "IX_PartInstances_UsageId",
                table: "PartInstances");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_StockId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "StockTypeId",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "UsageId",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "MinQuantity",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SafeQuantity",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PartId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Alerts");

            migrationBuilder.AddColumn<int>(
                name: "UsageId",
                table: "Parts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartTypeId",
                table: "PartInstances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartInstanceId",
                table: "Alerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "Discriminator", "Name" },
                values: new object[] { "", "PartType", "1x2-short" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Discriminator", "Name" },
                values: new object[] { "PartType", "1x2-long" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Description", "Discriminator", "Name" },
                values: new object[] { "Used only for System B01", "PartType", "System B01" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Used on A Systems", "A Systems" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Used on B Systems", "B Systems" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Used on C Systems", "C Systems" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Discriminator", "Name" },
                values: new object[,]
                {
                    { 39, "Used only for System C11", "PartType", "System C11" },
                    { 38, "Used only for System C10", "PartType", "System C10" },
                    { 37, "Used only for System C09", "PartType", "System C09" },
                    { 36, "Used only for System C08", "PartType", "System C08" },
                    { 35, "Used only for System C07", "PartType", "System C07" },
                    { 34, "Used only for System C06", "PartType", "System C06" },
                    { 33, "Used only for System C05", "PartType", "System C05" },
                    { 32, "Used only for System C04", "PartType", "System C04" },
                    { 31, "Used only for System C03", "PartType", "System C03" },
                    { 30, "Used only for System C02", "PartType", "System C02" },
                    { 28, "Used only for System A07", "PartType", "System A07" },
                    { 27, "Used only for System A06", "PartType", "System A06" },
                    { 26, "Used only for System A05", "PartType", "System A05" },
                    { 25, "Used only for System A04", "PartType", "System A04" },
                    { 24, "Used only for System A03", "PartType", "System A03" },
                    { 23, "Used only for System A02", "PartType", "System A02" },
                    { 22, "Used only for System A01", "PartType", "System A01" },
                    { 21, "Used only for System B07", "PartType", "System B07" },
                    { 20, "Used only for System B06", "PartType", "System B06" },
                    { 19, "Used only for System B05", "PartType", "System B05" },
                    { 18, "Used only for System B04", "PartType", "System B04" },
                    { 17, "Used only for System B03", "PartType", "System B03" },
                    { 29, "Used only for System C01", "PartType", "System C01" },
                    { 16, "Used only for System B02", "PartType", "System B02" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_UsageId",
                table: "Parts",
                column: "UsageId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_PartTypeId",
                table: "PartInstances",
                column: "PartTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_PartInstanceId",
                table: "Alerts",
                column: "PartInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_PartInstances_PartInstanceId",
                table: "Alerts",
                column: "PartInstanceId",
                principalTable: "PartInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Categories_PartTypeId",
                table: "PartInstances",
                column: "PartTypeId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_UsageId",
                table: "Parts",
                column: "UsageId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
