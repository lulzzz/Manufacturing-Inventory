using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class partInstanceRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstanceParameters_PartInstances_PartInstanceId",
                table: "InstanceParameters");

            migrationBuilder.DropIndex(
                name: "IX_InstanceParameters_PartInstanceId",
                table: "InstanceParameters");

            migrationBuilder.DropColumn(
                name: "CostCalcMethod",
                table: "PartInstances");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateInstalled",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRemoved",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GrossWeight",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MeasuredWeight",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NetWeight",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "PartInstances",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "PartInstances",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_InstanceParameters_PartInstances_PartInstanceId",
                table: "InstanceParameters",
                column: "PartInstanceId",
                principalTable: "PartInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstanceParameters_PartInstances_PartInstanceId",
                table: "InstanceParameters");

            migrationBuilder.DropColumn(
                name: "DateInstalled",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "DateRemoved",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "MeasuredWeight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PartInstances");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PartInstances");

            migrationBuilder.AddColumn<int>(
                name: "CostCalcMethod",
                table: "PartInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InstanceParameters_PartInstanceId",
                table: "InstanceParameters",
                column: "PartInstanceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstanceParameters_PartInstances_PartInstanceId",
                table: "InstanceParameters",
                column: "PartInstanceId",
                principalTable: "PartInstances",
                principalColumn: "Id");
        }
    }
}
