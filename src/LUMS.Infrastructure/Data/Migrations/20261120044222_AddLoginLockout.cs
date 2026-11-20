using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LUMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginLockout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KhoaDangNhapDen",
                table: "TAI_KHOAN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLanSaiMatKhau",
                table: "TAI_KHOAN",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KhoaDangNhapDen",
                table: "TAI_KHOAN");

            migrationBuilder.DropColumn(
                name: "SoLanSaiMatKhau",
                table: "TAI_KHOAN");
        }
    }
}
