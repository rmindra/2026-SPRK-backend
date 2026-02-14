using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPRK.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Ruang Theater Pascasarjana");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Ruang D4 Theater");
        }
    }
}
