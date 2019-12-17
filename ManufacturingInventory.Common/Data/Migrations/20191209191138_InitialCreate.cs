using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Common.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distributor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    Power = table.Column<int>(nullable: false),
                    Exponent = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    OgranizationId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true),
                    UsageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parts_Category_OgranizationId",
                        column: x => x.OgranizationId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parts_Category_UsageId",
                        column: x => x.UsageId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parts_Locations_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    DistributorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contacts_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    StorePassword = table.Column<bool>(nullable: false),
                    EncryptedPassword = table.Column<string>(nullable: true),
                    Key = table.Column<byte[]>(nullable: true),
                    IV = table.Column<byte[]>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PermissionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartInstances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    SkuNumber = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    MinQuantity = table.Column<int>(nullable: false),
                    SafeQuantity = table.Column<int>(nullable: false),
                    SerialNumber = table.Column<string>(nullable: true),
                    BatchNumber = table.Column<string>(nullable: true),
                    IsResuable = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PartId = table.Column<int>(nullable: false),
                    PartTypeId = table.Column<int>(nullable: true),
                    ConditionId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartInstances_Category_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartInstances_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartInstances_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartInstances_Category_PartTypeId",
                        column: x => x.PartTypeId,
                        principalTable: "Category",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartManufacturers",
                columns: table => new
                {
                    PartId = table.Column<int>(nullable: false),
                    ManufacturerId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartManufacturers", x => new { x.PartId, x.ManufacturerId });
                    table.ForeignKey(
                        name: "FK_PartManufacturers_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartManufacturers_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    In = table.Column<DateTime>(nullable: false),
                    Out = table.Column<DateTime>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Session_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEnabled = table.Column<bool>(nullable: false),
                    AlertTime = table.Column<DateTime>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PartInstanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_PartInstances_PartInstanceId",
                        column: x => x.PartInstanceId,
                        principalTable: "PartInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstanceParameters",
                columns: table => new
                {
                    ParameterId = table.Column<int>(nullable: false),
                    PartInstanceId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false),
                    MinValue = table.Column<double>(nullable: false),
                    SafeValue = table.Column<double>(nullable: false),
                    Tracked = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanceParameters", x => new { x.PartInstanceId, x.ParameterId });
                    table.ForeignKey(
                        name: "FK_InstanceParameters_Parameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InstanceParameters_PartInstances_PartInstanceId",
                        column: x => x.PartInstanceId,
                        principalTable: "PartInstances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    VaildFrom = table.Column<DateTime>(nullable: true),
                    ValidUntil = table.Column<DateTime>(nullable: true),
                    IsCurrent = table.Column<bool>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    MinOrder = table.Column<int>(nullable: false),
                    LeadTime = table.Column<double>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DistributorId = table.Column<int>(nullable: false),
                    PartInstanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Price_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Price_PartInstances_PartInstanceId",
                        column: x => x.PartInstanceId,
                        principalTable: "PartInstances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    InventoryAction = table.Column<int>(nullable: false),
                    IsReturning = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SessionId = table.Column<int>(nullable: false),
                    PartInstanceId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    InstanceParameterValue = table.Column<double>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    WarehouseId = table.Column<int>(nullable: true),
                    ConsumerId = table.Column<int>(nullable: true),
                    ReturningTransactionId = table.Column<int>(nullable: true),
                    OutgoingTransactionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Locations_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Locations_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Transactions_OutgoingTransactionId",
                        column: x => x.OutgoingTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_PartInstances_PartInstanceId",
                        column: x => x.PartInstanceId,
                        principalTable: "PartInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAlerts",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    AlertId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlerts", x => new { x.UserId, x.AlertId });
                    table.ForeignKey(
                        name: "FK_UserAlerts_Alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAlerts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    ValidThough = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SourceReference = table.Column<string>(nullable: true),
                    FileReference = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    Expires = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PartId = table.Column<int>(nullable: true),
                    DistributorId = table.Column<int>(nullable: true),
                    ManufacturerId = table.Column<int>(nullable: true),
                    PriceId = table.Column<int>(nullable: true),
                    PartInstanceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Distributor_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachments_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachments_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachments_PartInstances_PartInstanceId",
                        column: x => x.PartInstanceId,
                        principalTable: "PartInstances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attachments_Price_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Price",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_PartInstanceId",
                table: "Alerts",
                column: "PartInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_DistributorId",
                table: "Attachments",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ManufacturerId",
                table: "Attachments",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PartId",
                table: "Attachments",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PartInstanceId",
                table: "Attachments",
                column: "PartInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_PriceId",
                table: "Attachments",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DistributorId",
                table: "Contacts",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ManufacturerId",
                table: "Contacts",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceParameters_ParameterId",
                table: "InstanceParameters",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceParameters_PartInstanceId",
                table: "InstanceParameters",
                column: "PartInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_UnitId",
                table: "Parameters",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_ConditionId",
                table: "PartInstances",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_LocationId",
                table: "PartInstances",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_PartId",
                table: "PartInstances",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_PartTypeId",
                table: "PartInstances",
                column: "PartTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartManufacturers_ManufacturerId",
                table: "PartManufacturers",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_OgranizationId",
                table: "Parts",
                column: "OgranizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_UsageId",
                table: "Parts",
                column: "UsageId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_WarehouseId",
                table: "Parts",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_DistributorId",
                table: "Price",
                column: "DistributorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Price_PartInstanceId",
                table: "Price",
                column: "PartInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Session_UserId",
                table: "Session",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WarehouseId",
                table: "Transactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ConsumerId",
                table: "Transactions",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OutgoingTransactionId",
                table: "Transactions",
                column: "OutgoingTransactionId",
                unique: true,
                filter: "[OutgoingTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PartInstanceId",
                table: "Transactions",
                column: "PartInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SessionId",
                table: "Transactions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAlerts_AlertId",
                table: "UserAlerts",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PermissionId",
                table: "Users",
                column: "PermissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "InstanceParameters");

            migrationBuilder.DropTable(
                name: "PartManufacturers");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserAlerts");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Distributor");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PartInstances");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
