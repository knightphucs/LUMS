using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> b)
    {
        b.ToTable("GIAO_DICH_MUON_TRA");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaGiaoDich");
        b.Property(x => x.CopyId).HasColumnName("MaBanSao").HasMaxLength(30).IsUnicode(false);
        b.Property(x => x.ReaderId).HasColumnName("MaDocGia");
        b.Property(x => x.LibrarianId).HasColumnName("MaThuThuXuLy");
        b.Property(x => x.BorrowedAt).HasColumnName("NgayMuon");
        b.Property(x => x.DueDate).HasColumnName("NgayHetHan");
        b.Property(x => x.ReturnedDate).HasColumnName("NgayTraThucTe");
        b.Property(x => x.RenewalCount).HasColumnName("SoLanGiaHan").IsRequired();
        b.Property(x => x.Status).HasColumnName("TrangThai").HasConversion<string>().HasMaxLength(20).IsUnicode(false);

        b.HasOne(x => x.Copy).WithMany(c => c.Loans).HasForeignKey(x => x.CopyId);
        b.HasOne(x => x.Librarian).WithMany().HasForeignKey(x => x.LibrarianId);

        b.HasIndex(x => new { x.ReaderId, x.Status });
    }
}
