using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebQLPT.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ChuTroId",
                table: "Users",
                column: "ChuTroId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_KhachThueId",
                table: "Users",
                column: "KhachThueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ChuTros_ChuTroId",
                table: "Users",
                column: "ChuTroId",
                principalTable: "ChuTros",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_KhachThues_KhachThueId",
                table: "Users",
                column: "KhachThueId",
                principalTable: "KhachThues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ChuTros_ChuTroId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_KhachThues_KhachThueId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ChuTroId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_KhachThueId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
