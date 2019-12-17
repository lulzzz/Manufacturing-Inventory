using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class fixingPluralization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Distributor_DistributorId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Price_PriceId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Distributor_DistributorId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Category_ConditionId",
                table: "PartInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Category_PartTypeId",
                table: "PartInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Category_OgranizationId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Category_UsageId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Price_Distributor_DistributorId",
                table: "Price");

            migrationBuilder.DropForeignKey(
                name: "FK_Price_PartInstances_PartInstanceId",
                table: "Price");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Price",
                table: "Price");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Distributor",
                table: "Distributor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Price",
                newName: "Prices");

            migrationBuilder.RenameTable(
                name: "Distributor",
                newName: "Distributors");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Price_PartInstanceId",
                table: "Prices",
                newName: "IX_Prices_PartInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Price_DistributorId",
                table: "Prices",
                newName: "IX_Prices_DistributorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prices",
                table: "Prices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Distributors",
                table: "Distributors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Distributors_DistributorId",
                table: "Attachments",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Prices_PriceId",
                table: "Attachments",
                column: "PriceId",
                principalTable: "Prices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Distributors_DistributorId",
                table: "Contacts",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Categories_ConditionId",
                table: "PartInstances",
                column: "ConditionId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Categories_PartTypeId",
                table: "PartInstances",
                column: "PartTypeId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_OgranizationId",
                table: "Parts",
                column: "OgranizationId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_UsageId",
                table: "Parts",
                column: "UsageId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_Distributors_DistributorId",
                table: "Prices",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_PartInstances_PartInstanceId",
                table: "Prices",
                column: "PartInstanceId",
                principalTable: "PartInstances",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Distributors_DistributorId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Prices_PriceId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Distributors_DistributorId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Categories_ConditionId",
                table: "PartInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_PartInstances_Categories_PartTypeId",
                table: "PartInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_OgranizationId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_UsageId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_Distributors_DistributorId",
                table: "Prices");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_PartInstances_PartInstanceId",
                table: "Prices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prices",
                table: "Prices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Distributors",
                table: "Distributors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Prices",
                newName: "Price");

            migrationBuilder.RenameTable(
                name: "Distributors",
                newName: "Distributor");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_PartInstanceId",
                table: "Price",
                newName: "IX_Price_PartInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_DistributorId",
                table: "Price",
                newName: "IX_Price_DistributorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Price",
                table: "Price",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Distributor",
                table: "Distributor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Distributor_DistributorId",
                table: "Attachments",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Price_PriceId",
                table: "Attachments",
                column: "PriceId",
                principalTable: "Price",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Distributor_DistributorId",
                table: "Contacts",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Category_ConditionId",
                table: "PartInstances",
                column: "ConditionId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartInstances_Category_PartTypeId",
                table: "PartInstances",
                column: "PartTypeId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Category_OgranizationId",
                table: "Parts",
                column: "OgranizationId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Category_UsageId",
                table: "Parts",
                column: "UsageId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Price_Distributor_DistributorId",
                table: "Price",
                column: "DistributorId",
                principalTable: "Distributor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Price_PartInstances_PartInstanceId",
                table: "Price",
                column: "PartInstanceId",
                principalTable: "PartInstances",
                principalColumn: "Id");
        }
    }
}
