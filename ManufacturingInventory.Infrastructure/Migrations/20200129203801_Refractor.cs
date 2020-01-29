using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class Refractor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_OgranizationId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_OgranizationId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "OgranizationId",
                table: "Parts");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Parts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parts_OrganizationId",
                table: "Parts",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_OrganizationId",
                table: "Parts",
                column: "OrganizationId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_OrganizationId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_OrganizationId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Parts");

            migrationBuilder.AddColumn<int>(
                name: "OgranizationId",
                table: "Parts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parts_OgranizationId",
                table: "Parts",
                column: "OgranizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_OgranizationId",
                table: "Parts",
                column: "OgranizationId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
