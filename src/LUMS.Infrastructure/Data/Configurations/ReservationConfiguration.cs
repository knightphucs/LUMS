using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> b)
    {
        b.ToTable("DAT_TRUOC");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaDatTruoc");
        b.Property(x => x.BookTitleId).HasColumnName("MaDauSach");
        b.Property(x => x.ReaderId).HasColumnName("MaDocGia");
        b.Property(x => x.ReservedAt).HasColumnName("NgayDat");
        b.Property(x => x.QueuePosition).HasColumnName("ThuTuHangCho");
        b.Property(x => x.Status).HasColumnName("TrangThai").HasConversion<string>().HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.HoldUntil).HasColumnName("HanGiuCho");

        b.HasOne(x => x.BookTitle).WithMany(t => t.Reservations).HasForeignKey(x => x.BookTitleId);
        b.HasOne(x => x.Reader).WithMany().HasForeignKey(x => x.ReaderId);
    }
}
