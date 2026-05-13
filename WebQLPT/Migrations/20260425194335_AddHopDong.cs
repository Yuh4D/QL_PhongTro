using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebQLPT.Migrations
{
    /// <inheritdoc />
    public partial class AddHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HopDongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhongTroId = table.Column<int>(type: "int", nullable: false),
                    KhachThueId = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HopDongs_KhachThues_KhachThueId",
                        column: x => x.KhachThueId,
                        principalTable: "KhachThues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HopDongs_PhongTros_PhongTroId",
                        column: x => x.PhongTroId,
                        principalTable: "PhongTros",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_KhachThueId",
                table: "HopDongs",
                column: "KhachThueId");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_PhongTroId",
                table: "HopDongs",
                column: "PhongTroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HopDongs");
        }
    }
}
