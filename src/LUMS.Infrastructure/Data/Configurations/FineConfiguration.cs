using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> b)
    {
        b.ToTable("PHIEU_PHAT");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaPhieuPhat");
        b.Property(x => x.LoanId).HasColumnName("MaGiaoDich");
        b.Property(x => x.CopyId).HasColumnName("MaBanSao").HasMaxLength(30).IsUnicode(false);
        b.Property(x => x.ReaderId).HasColumnName("MaDocGia");
        b.Property(x => x.LibrarianId).HasColumnName("MaThuThuXuLy");
        b.Property(x => x.Type).HasColumnName("LoaiPhat").HasConversion<string>().HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Amount).HasColumnName("SoTien").HasColumnType("decimal(12,0)");
        b.Property(x => x.Note).HasColumnName("GhiChu");
        b.Property(x => x.HandledAt).HasColumnName("NgayXuLy");
        b.Property(x => x.PaymentStatus).HasColumnName("TrangThaiThanhToan").HasConversion<string>().HasMaxLength(20).IsUnicode(false);

        b.HasOne(x => x.Loan).WithMany().HasForeignKey(x => x.LoanId);
        b.HasOne(x => x.Copy).WithMany().HasForeignKey(x => x.CopyId);
        b.HasOne(x => x.Reader).WithMany().HasForeignKey(x => x.ReaderId);
        b.HasOne(x => x.Librarian).WithMany().HasForeignKey(x => x.LibrarianId);
    }
}
