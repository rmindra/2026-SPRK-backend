using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SPRK.Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
