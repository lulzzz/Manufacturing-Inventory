using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class updateSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Distributors",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 7, "", "K.J Lesker" },
                    { 8, "", "Fisher" },
                    { 9, "", "SVC" },
                    { 10, "", "lljin" },
                    { 11, "", "Nouryon" },
                    { 12, "", "SAFC" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
