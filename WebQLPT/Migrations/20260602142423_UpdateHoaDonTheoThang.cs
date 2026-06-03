using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebQLPT.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHoaDonTheoThang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChiSoDienCu",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChiSoDienMoi",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChiSoNuocCu",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChiSoNuocMoi",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Nam",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Thang",
                table: "HoaDons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TienDien",
                table: "HoaDons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TienNuoc",
                table: "HoaDons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TienPhong",
                table: "HoaDons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiSoDienCu",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "ChiSoDienMoi",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "ChiSoNuocCu",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "ChiSoNuocMoi",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "Nam",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "Thang",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "TienDien",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "TienNuoc",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "TienPhong",
                table: "HoaDons");
        }
    }
}
