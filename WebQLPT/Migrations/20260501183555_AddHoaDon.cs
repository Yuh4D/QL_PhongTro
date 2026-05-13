using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebQLPT.Migrations
{
    /// <inheritdoc />
    public partial class AddHoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachThues_PhongTros_PhongTroId",
                table: "KhachThues");

            migrationBuilder.RenameColumn(
                name: "KhachThueID",
                table: "Users",
                newName: "KhachThueId");

            migrationBuilder.RenameColumn(
                name: "ChuTroID",
                table: "Users",
                newName: "ChuTroId");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenPhong",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenKhach",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayThue",
                table: "KhachThues",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CCCD",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "DangO",
                table: "KhachThues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKetThuc",
                table: "KhachThues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "HopDongs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenChuTro",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhongTroId = table.Column<int>(type: "int", nullable: false),
                    KhachThueId = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachThues_KhachThueId",
                        column: x => x.KhachThueId,
                        principalTable: "KhachThues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoaDons_PhongTros_PhongTroId",
                        column: x => x.PhongTroId,
                        principalTable: "PhongTros",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonChiTiets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoaDonId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    HeSo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiets_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiets_HoaDonId",
                table: "HoaDonChiTiets",
                column: "HoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_KhachThueId",
                table: "HoaDons",
                column: "KhachThueId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_PhongTroId",
                table: "HoaDons",
                column: "PhongTroId");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachThues_PhongTros_PhongTroId",
                table: "KhachThues",
                column: "PhongTroId",
                principalTable: "PhongTros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachThues_PhongTros_PhongTroId",
                table: "KhachThues");

            migrationBuilder.DropTable(
                name: "HoaDonChiTiets");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropColumn(
                name: "DangO",
                table: "KhachThues");

            migrationBuilder.DropColumn(
                name: "NgayKetThuc",
                table: "KhachThues");

            migrationBuilder.RenameColumn(
                name: "KhachThueId",
                table: "Users",
                newName: "KhachThueID");

            migrationBuilder.RenameColumn(
                name: "ChuTroId",
                table: "Users",
                newName: "ChuTroID");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenPhong",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "PhongTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenKhach",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayThue",
                table: "KhachThues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CCCD",
                table: "KhachThues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "HopDongs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenChuTro",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "ChuTros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KhachThues_PhongTros_PhongTroId",
                table: "KhachThues",
                column: "PhongTroId",
                principalTable: "PhongTros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
