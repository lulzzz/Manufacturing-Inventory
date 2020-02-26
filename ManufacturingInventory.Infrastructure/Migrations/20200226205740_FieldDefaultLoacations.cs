using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class FieldDefaultLoacations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefualt",
                table: "Locations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefualt",
                table: "Locations");
        }
    }
}
