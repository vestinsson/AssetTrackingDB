using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetTrackingDB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    OfficeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    ProcessorType = table.Column<string>(type: "TEXT", nullable: true),
                    MemoryGB = table.Column<int>(type: "INTEGER", nullable: true),
                    StorageCapacity = table.Column<string>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: true),
                    LenovoLaptop_ProcessorType = table.Column<string>(type: "TEXT", nullable: true),
                    LenovoLaptop_MemoryGB = table.Column<int>(type: "INTEGER", nullable: true),
                    MacBook_ProcessorType = table.Column<string>(type: "TEXT", nullable: true),
                    MacBook_MemoryGB = table.Column<int>(type: "INTEGER", nullable: true),
                    NokiaPhone_StorageCapacity = table.Column<string>(type: "TEXT", nullable: true),
                    NokiaPhone_Color = table.Column<string>(type: "TEXT", nullable: true),
                    SamsungPhone_StorageCapacity = table.Column<string>(type: "TEXT", nullable: true),
                    SamsungPhone_Color = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asset_Offices_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Offices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetUsers",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUsers", x => new { x.AssetId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AssetUsers_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "Country", "Name" },
                values: new object[,]
                {
                    { 1, 0, "San Francisco HQ" },
                    { 2, 1, "Berlin Office" },
                    { 3, 2, "Stockholm Office" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Alice" },
                    { 2, "Robert" },
                    { 3, "Carl" },
                    { 4, "Diana" },
                    { 5, "Eric" }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "Discriminator", "Manufacturer", "MacBook_MemoryGB", "Model", "OfficeId", "Price", "MacBook_ProcessorType", "PurchaseDate" },
                values: new object[,]
                {
                    { 1, "MacBook", "Apple", 16, "MacBook Pro 16\"", 1, 2499.99m, "M1 Pro", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "MacBook", "Apple", 8, "MacBook Air", 2, 1299.99m, "M1", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "Discriminator", "Manufacturer", "MemoryGB", "Model", "OfficeId", "Price", "ProcessorType", "PurchaseDate" },
                values: new object[,]
                {
                    { 3, "AsusLaptop", "Asus", 32, "Zephyrus G14", 3, 1799.99m, "AMD Ryzen 9", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "AsusLaptop", "Asus", 16, "ZenBook 14", 1, 1099.99m, "Intel Core i7", new DateTime(2021, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "Discriminator", "Manufacturer", "LenovoLaptop_MemoryGB", "Model", "OfficeId", "Price", "LenovoLaptop_ProcessorType", "PurchaseDate" },
                values: new object[,]
                {
                    { 5, "LenovoLaptop", "Lenovo", 16, "ThinkPad X1", 2, 1899.99m, "Intel Core i7", new DateTime(2022, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "LenovoLaptop", "Lenovo", 12, "Yoga 9i", 3, 1499.99m, "Intel Core i5", new DateTime(2022, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "Color", "Discriminator", "Manufacturer", "Model", "OfficeId", "Price", "PurchaseDate", "StorageCapacity" },
                values: new object[,]
                {
                    { 7, "Graphite", "Iphone", "Apple", "iPhone 13 Pro", 1, 1099.99m, new DateTime(2021, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "256GB" },
                    { 8, "Blue", "Iphone", "Apple", "iPhone 12", 3, 799.99m, new DateTime(2021, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "128GB" }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "SamsungPhone_Color", "Discriminator", "Manufacturer", "Model", "OfficeId", "Price", "PurchaseDate", "SamsungPhone_StorageCapacity" },
                values: new object[,]
                {
                    { 9, "Phantom Black", "SamsungPhone", "Samsung", "Galaxy S21", 2, 1199.99m, new DateTime(2021, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "512GB" },
                    { 10, "Awesome Blue", "SamsungPhone", "Samsung", "Galaxy A52", 3, 499.99m, new DateTime(2022, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "128GB" }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "NokiaPhone_Color", "Discriminator", "Manufacturer", "Model", "OfficeId", "Price", "PurchaseDate", "NokiaPhone_StorageCapacity" },
                values: new object[,]
                {
                    { 11, "Polar Night", "NokiaPhone", "Nokia", "8.3 5G", 1, 599.99m, new DateTime(2022, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "128GB" },
                    { 12, "Night", "NokiaPhone", "Nokia", "G20", 2, 249.99m, new DateTime(2022, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "64GB" }
                });

            migrationBuilder.InsertData(
                table: "AssetUsers",
                columns: new[] { "AssetId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 5 },
                    { 3, 2 },
                    { 3, 4 },
                    { 4, 2 },
                    { 4, 5 },
                    { 5, 1 },
                    { 5, 3 },
                    { 6, 2 },
                    { 6, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OfficeId",
                table: "Asset",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetUsers_UserId",
                table: "AssetUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetUsers");

            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Offices");
        }
    }
}
