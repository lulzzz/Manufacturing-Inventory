using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ManufacturingInventory.Infrastructure.Migrations
{
    public partial class ReworkInfrastructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BubblerParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NetWeight = table.Column<double>(nullable: false),
                    Tare = table.Column<double>(nullable: false),
                    GrossWeight = table.Column<double>(nullable: false),
                    Measured = table.Column<double>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    DateInstalled = table.Column<DateTime>(nullable: true),
                    DateRemoved = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BubblerParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
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
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distributors",
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
                    table.PrimaryKey("PK_Distributors", x => x.Id);
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
                    HoldsBubblers = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    OgranizationId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true),
                    UsageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parts_Categories_OgranizationId",
                        column: x => x.OgranizationId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parts_Categories_UsageId",
                        column: x => x.UsageId,
                        principalTable: "Categories",
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
                        name: "FK_Contacts_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
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
                    UnitCost = table.Column<double>(nullable: false),
                    TotalCost = table.Column<double>(nullable: false),
                    SerialNumber = table.Column<string>(nullable: true),
                    BatchNumber = table.Column<string>(nullable: true),
                    CostReported = table.Column<bool>(nullable: false),
                    IsResuable = table.Column<bool>(nullable: false),
                    IsBubbler = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    PartId = table.Column<int>(nullable: false),
                    PartTypeId = table.Column<int>(nullable: true),
                    ConditionId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: false),
                    BubblerParameterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartInstances_BubblerParameters_BubblerParameterId",
                        column: x => x.BubblerParameterId,
                        principalTable: "BubblerParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartInstances_Categories_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "Categories",
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
                        name: "FK_PartInstances_Categories_PartTypeId",
                        column: x => x.PartTypeId,
                        principalTable: "Categories",
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
                name: "Sessions",
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
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    VaildFrom = table.Column<DateTime>(nullable: true),
                    ValidUntil = table.Column<DateTime>(nullable: true),
                    IsCurrent = table.Column<bool>(nullable: false),
                    UnitCost = table.Column<double>(nullable: false),
                    MinOrder = table.Column<int>(nullable: false),
                    LeadTime = table.Column<double>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    DistributorId = table.Column<int>(nullable: false),
                    PartInstanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prices_PartInstances_PartInstanceId",
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
                    Quantity = table.Column<int>(nullable: false),
                    ParameterValue = table.Column<double>(nullable: false),
                    IsReturning = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UnitCost = table.Column<double>(nullable: false),
                    TotalCost = table.Column<double>(nullable: false),
                    SessionId = table.Column<int>(nullable: false),
                    PartInstanceId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: true),
                    ReferenceTransactionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_PartInstances_PartInstanceId",
                        column: x => x.PartInstanceId,
                        principalTable: "PartInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Transactions_ReferenceTransactionId",
                        column: x => x.ReferenceTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
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
                        name: "FK_Attachments_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
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
                        name: "FK_Attachments_Prices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Prices",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Full Inventory Privileges and User Control", "InventoryAdminAccount" },
                    { 2, "Inventory View Only", "InventoryUserAccount" },
                    { 3, "Full Inventory Privileges", "InventoryUserFullAccount" },
                    { 4, "Inventory Check In/Check Out/Create", "InventoryUserLimitedAccount" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EncryptedPassword", "Extension", "FirstName", "IV", "Key", "LastName", "PermissionId", "StorePassword", "UserName" },
                values: new object[] { 1, null, null, null, "Andrew", null, null, "Elmendorf", 1, false, "AElmendo" });

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
                name: "IX_Parameters_UnitId",
                table: "Parameters",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PartInstances_BubblerParameterId",
                table: "PartInstances",
                column: "BubblerParameterId",
                unique: true,
                filter: "[BubblerParameterId] IS NOT NULL");

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
                name: "IX_Prices_DistributorId",
                table: "Prices",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PartInstanceId",
                table: "Prices",
                column: "PartInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LocationId",
                table: "Transactions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PartInstanceId",
                table: "Transactions",
                column: "PartInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReferenceTransactionId",
                table: "Transactions",
                column: "ReferenceTransactionId",
                unique: true,
                filter: "[ReferenceTransactionId] IS NOT NULL");

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
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Distributors");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PartInstances");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "BubblerParameters");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
