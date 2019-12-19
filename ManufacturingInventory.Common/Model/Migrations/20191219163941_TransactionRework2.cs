using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class TransactionRework2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Transactions_OutgoingTransactionId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_OutgoingTransactionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InstanceParameterValue",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OutgoingTransactionId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "ReferenceTransactionId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReferenceTransactionId",
                table: "Transactions",
                column: "ReferenceTransactionId",
                unique: true,
                filter: "[ReferenceTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Transactions_ReferenceTransactionId",
                table: "Transactions",
                column: "ReferenceTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Transactions_ReferenceTransactionId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReferenceTransactionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReferenceTransactionId",
                table: "Transactions");

            migrationBuilder.AddColumn<double>(
                name: "InstanceParameterValue",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "OutgoingTransactionId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OutgoingTransactionId",
                table: "Transactions",
                column: "OutgoingTransactionId",
                unique: true,
                filter: "[OutgoingTransactionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Transactions_OutgoingTransactionId",
                table: "Transactions",
                column: "OutgoingTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
