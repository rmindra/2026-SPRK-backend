using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SPRK.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTableAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "CreatedAt", "Description", "IsAvailable", "IsDeleted", "Location", "Name" },
                values: new object[,]
                {
                    { 1, 90, new DateTime(2026, 2, 14, 11, 24, 43, 589, DateTimeKind.Utc).AddTicks(2624), "Lengkap dengan proyektor dan sound system", true, false, "Gedung D3, HH 101", "Ruang D3 Teater" },
                    { 2, 90, new DateTime(2026, 2, 14, 11, 24, 43, 589, DateTimeKind.Utc).AddTicks(3024), "Lengkap dengan proyektor, sound system, dan kursi empuk", true, false, "Gedung Pascasarjana, Lantai 6", "Ruang D4 Theater" },
                    { 3, 600, new DateTime(2026, 2, 14, 11, 24, 43, 589, DateTimeKind.Utc).AddTicks(3026), "Layar, Sound system, dan Ruangan super besar", true, false, "Gedung Pascasarjana, Lantai 6", "Auditorium" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
