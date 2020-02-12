using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class PartInstanceCommentsDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PartInstances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PartInstances");
        }
    }
}
