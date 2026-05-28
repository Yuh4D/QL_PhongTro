using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebQLPT.Migrations
{
    /// <inheritdoc />
    public partial class AddDieuKhoanThemToHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ChuTroId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_KhachThueId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "DieuKhoanThem",
                table: "HopDongs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ChuTroId",
                table: "Users",
                column: "ChuTroId",
                unique: true,
                filter: "[ChuTroId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_KhachThueId",
                table: "Users",
                column: "KhachThueId",
                unique: true,
                filter: "[KhachThueId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ChuTroId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_KhachThueId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DieuKhoanThem",
                table: "HopDongs");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ChuTroId",
                table: "Users",
                column: "ChuTroId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_KhachThueId",
                table: "Users",
                column: "KhachThueId");
        }
    }
}
