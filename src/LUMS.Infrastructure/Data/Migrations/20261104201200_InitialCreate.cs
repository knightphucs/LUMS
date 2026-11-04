using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LUMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DAU_SACH",
                columns: table => new
                {
                    MaDauSach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSach = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TacGia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TheLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhBia = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ViTriKeMacDinh = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAU_SACH", x => x.MaDauSach);
                });

            migrationBuilder.CreateTable(
                name: "TAI_KHOAN",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MSSV_MaNV = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    MatKhauHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    VaiTro = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    LoaiDocGia = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    TrangThai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAI_KHOAN", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "BAN_SAO",
                columns: table => new
                {
                    MaBanSao = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    MaDauSach = table.Column<int>(type: "int", nullable: false),
                    ViTriKe = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TinhTrang = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BAN_SAO", x => x.MaBanSao);
                    table.ForeignKey(
                        name: "FK_BAN_SAO_DAU_SACH_MaDauSach",
                        column: x => x.MaDauSach,
                        principalTable: "DAU_SACH",
                        principalColumn: "MaDauSach",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CAU_HINH_QUY_DINH",
                columns: table => new
                {
                    MaCauHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNgayMuonToiDa = table.Column<int>(type: "int", nullable: false),
                    SoSachMuonToiDa = table.Column<int>(type: "int", nullable: false),
                    SoDatTruocToiDa = table.Column<int>(type: "int", nullable: false),
                    MucPhatMoiNgay = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    NguoiCapNhat = table.Column<int>(type: "int", nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAU_HINH_QUY_DINH", x => x.MaCauHinh);
                    table.ForeignKey(
                        name: "FK_CAU_HINH_QUY_DINH_TAI_KHOAN_NguoiCapNhat",
                        column: x => x.NguoiCapNhat,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DAT_TRUOC",
                columns: table => new
                {
                    MaDatTruoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDauSach = table.Column<int>(type: "int", nullable: false),
                    MaDocGia = table.Column<int>(type: "int", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThuTuHangCho = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    HanGiuCho = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAT_TRUOC", x => x.MaDatTruoc);
                    table.ForeignKey(
                        name: "FK_DAT_TRUOC_DAU_SACH_MaDauSach",
                        column: x => x.MaDauSach,
                        principalTable: "DAU_SACH",
                        principalColumn: "MaDauSach",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DAT_TRUOC_TAI_KHOAN_MaDocGia",
                        column: x => x.MaDocGia,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_HE_THONG",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DoiTuongLienQuan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_HE_THONG", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_HE_THONG_TAI_KHOAN_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GIAO_DICH_MUON_TRA",
                columns: table => new
                {
                    MaGiaoDich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBanSao = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    MaDocGia = table.Column<int>(type: "int", nullable: false),
                    MaThuThuXuLy = table.Column<int>(type: "int", nullable: true),
                    NgayMuon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayTraThucTe = table.Column<DateOnly>(type: "date", nullable: true),
                    SoLanGiaHan = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GIAO_DICH_MUON_TRA", x => x.MaGiaoDich);
                    table.ForeignKey(
                        name: "FK_GIAO_DICH_MUON_TRA_BAN_SAO_MaBanSao",
                        column: x => x.MaBanSao,
                        principalTable: "BAN_SAO",
                        principalColumn: "MaBanSao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GIAO_DICH_MUON_TRA_TAI_KHOAN_MaDocGia",
                        column: x => x.MaDocGia,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GIAO_DICH_MUON_TRA_TAI_KHOAN_MaThuThuXuLy",
                        column: x => x.MaThuThuXuLy,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PHIEU_PHAT",
                columns: table => new
                {
                    MaPhieuPhat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiaoDich = table.Column<int>(type: "int", nullable: true),
                    MaBanSao = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    MaDocGia = table.Column<int>(type: "int", nullable: false),
                    MaThuThuXuLy = table.Column<int>(type: "int", nullable: false),
                    LoaiPhat = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHIEU_PHAT", x => x.MaPhieuPhat);
                    table.ForeignKey(
                        name: "FK_PHIEU_PHAT_BAN_SAO_MaBanSao",
                        column: x => x.MaBanSao,
                        principalTable: "BAN_SAO",
                        principalColumn: "MaBanSao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PHIEU_PHAT_GIAO_DICH_MUON_TRA_MaGiaoDich",
                        column: x => x.MaGiaoDich,
                        principalTable: "GIAO_DICH_MUON_TRA",
                        principalColumn: "MaGiaoDich",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PHIEU_PHAT_TAI_KHOAN_MaDocGia",
                        column: x => x.MaDocGia,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PHIEU_PHAT_TAI_KHOAN_MaThuThuXuLy",
                        column: x => x.MaThuThuXuLy,
                        principalTable: "TAI_KHOAN",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BAN_SAO_MaDauSach_TinhTrang",
                table: "BAN_SAO",
                columns: new[] { "MaDauSach", "TinhTrang" });

            migrationBuilder.CreateIndex(
                name: "IX_CAU_HINH_QUY_DINH_NguoiCapNhat",
                table: "CAU_HINH_QUY_DINH",
                column: "NguoiCapNhat");

            migrationBuilder.CreateIndex(
                name: "IX_DAT_TRUOC_MaDauSach",
                table: "DAT_TRUOC",
                column: "MaDauSach");

            migrationBuilder.CreateIndex(
                name: "IX_DAT_TRUOC_MaDocGia",
                table: "DAT_TRUOC",
                column: "MaDocGia");

            migrationBuilder.CreateIndex(
                name: "IX_DAU_SACH_ISBN",
                table: "DAU_SACH",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DAU_SACH_TenSach",
                table: "DAU_SACH",
                column: "TenSach");

            migrationBuilder.CreateIndex(
                name: "IX_GIAO_DICH_MUON_TRA_MaBanSao",
                table: "GIAO_DICH_MUON_TRA",
                column: "MaBanSao");

            migrationBuilder.CreateIndex(
                name: "IX_GIAO_DICH_MUON_TRA_MaDocGia_TrangThai",
                table: "GIAO_DICH_MUON_TRA",
                columns: new[] { "MaDocGia", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_GIAO_DICH_MUON_TRA_MaThuThuXuLy",
                table: "GIAO_DICH_MUON_TRA",
                column: "MaThuThuXuLy");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_HE_THONG_MaTaiKhoan",
                table: "NHAT_KY_HE_THONG",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_HE_THONG_ThoiGian",
                table: "NHAT_KY_HE_THONG",
                column: "ThoiGian");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_PHAT_MaBanSao",
                table: "PHIEU_PHAT",
                column: "MaBanSao");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_PHAT_MaDocGia",
                table: "PHIEU_PHAT",
                column: "MaDocGia");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_PHAT_MaGiaoDich",
                table: "PHIEU_PHAT",
                column: "MaGiaoDich");

            migrationBuilder.CreateIndex(
                name: "IX_PHIEU_PHAT_MaThuThuXuLy",
                table: "PHIEU_PHAT",
                column: "MaThuThuXuLy");

            migrationBuilder.CreateIndex(
                name: "IX_TAI_KHOAN_Email",
                table: "TAI_KHOAN",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TAI_KHOAN_MSSV_MaNV",
                table: "TAI_KHOAN",
                column: "MSSV_MaNV",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CAU_HINH_QUY_DINH");

            migrationBuilder.DropTable(
                name: "DAT_TRUOC");

            migrationBuilder.DropTable(
                name: "NHAT_KY_HE_THONG");

            migrationBuilder.DropTable(
                name: "PHIEU_PHAT");

            migrationBuilder.DropTable(
                name: "GIAO_DICH_MUON_TRA");

            migrationBuilder.DropTable(
                name: "BAN_SAO");

            migrationBuilder.DropTable(
                name: "TAI_KHOAN");

            migrationBuilder.DropTable(
                name: "DAU_SACH");
        }
    }
}
