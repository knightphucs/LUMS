using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class CopyConfiguration : IEntityTypeConfiguration<Copy>
{
    public void Configure(EntityTypeBuilder<Copy> b)
    {
        b.ToTable("BAN_SAO");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaBanSao").HasMaxLength(30).IsUnicode(false).ValueGeneratedNever();
        b.Property(x => x.BookTitleId).HasColumnName("MaDauSach");
        b.Property(x => x.Shelf).HasColumnName("ViTriKe").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.Status).HasColumnName("TinhTrang").HasConversion<string>().HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.AddedAt).HasColumnName("NgayNhap");

        b.HasIndex(x => new { x.BookTitleId, x.Status });
    }
}
