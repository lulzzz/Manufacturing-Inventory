using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class MonthlySummaryAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlySummaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateGenerated = table.Column<DateTime>(nullable: false),
                    MonthStartDate = table.Column<DateTime>(nullable: false),
                    MonthStopDate = table.Column<DateTime>(nullable: false),
                    RowVersion = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlySummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartMonthlySummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthlySummaryId = table.Column<int>(nullable: false),
                    PartName = table.Column<string>(nullable: true),
                    InstanceName = table.Column<string>(nullable: true),
                    StartQuantity = table.Column<double>(nullable: false),
                    StartCost = table.Column<double>(nullable: false),
                    IncomingQuantity = table.Column<double>(nullable: false),
                    IncomingCost = table.Column<double>(nullable: false),
                    ProductionOutgoingQuantity = table.Column<double>(nullable: false),
                    ProductionOutgoingCost = table.Column<double>(nullable: false),
                    RndOutgoingQuantity = table.Column<double>(nullable: false),
                    RndOutgoingCost = table.Column<double>(nullable: false),
                    TotalOutgoingQuantity = table.Column<double>(nullable: false),
                    TotalOutgoingCost = table.Column<double>(nullable: false),
                    CurrentQuantity = table.Column<double>(nullable: false),
                    CurrentCost = table.Column<double>(nullable: false),
                    EndQuantity = table.Column<double>(nullable: false),
                    EndCost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartMonthlySummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartMonthlySummary_MonthlySummaries_MonthlySummaryId",
                        column: x => x.MonthlySummaryId,
                        principalTable: "MonthlySummaries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartMonthlySummary_MonthlySummaryId",
                table: "PartMonthlySummary",
                column: "MonthlySummaryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
