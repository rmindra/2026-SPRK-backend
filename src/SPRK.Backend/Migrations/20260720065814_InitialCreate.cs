using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SPRK.Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    BorrowerName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "CreatedAt", "Description", "IsAvailable", "IsDeleted", "Location", "Name" },
                values: new object[,]
                {
                    { 1, 90, new DateTime(2026, 2, 14, 11, 24, 0, 0, DateTimeKind.Utc), "Lengkap dengan proyektor dan sound system", true, false, "Gedung D3, HH 101", "Ruang D3 Teater" },
                    { 2, 90, new DateTime(2026, 2, 14, 11, 24, 0, 0, DateTimeKind.Utc), "Lengkap dengan proyektor, sound system, dan kursi empuk", true, false, "Gedung Pascasarjana, Lantai 6", "Ruang Theater Pascasarjana" },
                    { 3, 600, new DateTime(2026, 2, 14, 11, 24, 0, 0, DateTimeKind.Utc), "Layar, Sound system, dan Ruangan super besar", true, false, "Gedung Pascasarjana, Lantai 6", "Auditorium" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BorrowerName", "CreatedAt", "EndTime", "IsDeleted", "Purpose", "RoomId", "StartTime", "Status" },
                values: new object[] { 1, "Andi Setiawan", new DateTime(2026, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 18, 4, 0, 0, 0, DateTimeKind.Utc), false, "Seminar internal", 1, new DateTime(2026, 2, 18, 2, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BorrowerName", "CreatedAt", "EndTime", "IsDeleted", "Purpose", "RoomId", "StartTime" },
                values: new object[] { 2, "Rina Kurnia", new DateTime(2026, 2, 14, 12, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 19, 9, 0, 0, 0, DateTimeKind.Utc), false, "Pelatihan lab", 2, new DateTime(2026, 2, 19, 6, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BorrowerName", "CreatedAt", "EndTime", "IsDeleted", "Purpose", "RoomId", "StartTime", "Status" },
                values: new object[,]
                {
                    { 3, "Dosen TI", new DateTime(2026, 2, 14, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 20, 3, 0, 0, 0, DateTimeKind.Utc), false, "Kuliah umum", 3, new DateTime(2026, 2, 20, 1, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 4, "Mahasiswa TI", new DateTime(2026, 2, 14, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 21, 12, 0, 0, 0, DateTimeKind.Utc), false, "Diskusi tugas akhir", 1, new DateTime(2026, 2, 21, 10, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
