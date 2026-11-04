using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("TAI_KHOAN");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaTaiKhoan");
        b.Property(x => x.FullName).HasColumnName("HoTen").HasMaxLength(100).IsRequired();
        b.Property(x => x.Email).HasColumnName("Email").HasMaxLength(100).IsUnicode(false).IsRequired();
        b.Property(x => x.Code).HasColumnName("MSSV_MaNV").HasMaxLength(20).IsUnicode(false).IsRequired();
        b.Property(x => x.PasswordHash).HasColumnName("MatKhauHash").HasMaxLength(255).IsUnicode(false).IsRequired();
        b.Property(x => x.PhoneNumber).HasColumnName("SoDienThoai").HasMaxLength(15).IsUnicode(false);
        b.Property(x => x.Role).HasColumnName("VaiTro").HasConversion<string>().HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.ReaderType).HasColumnName("LoaiDocGia").HasConversion<string>().HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Status).HasColumnName("TrangThai").HasConversion<string>().HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.CreatedAt).HasColumnName("NgayTao");

        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.Code).IsUnique();

        b.HasMany(x => x.Loans).WithOne(l => l.Reader).HasForeignKey(l => l.ReaderId);
    }
}
