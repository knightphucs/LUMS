using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class BookTitleConfiguration : IEntityTypeConfiguration<BookTitle>
{
    public void Configure(EntityTypeBuilder<BookTitle> b)
    {
        b.ToTable("DAU_SACH");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaDauSach");
        b.Property(x => x.Title).HasColumnName("TenSach").HasMaxLength(200).IsRequired();
        b.Property(x => x.Author).HasColumnName("TacGia").HasMaxLength(150).IsRequired();
        b.Property(x => x.Category).HasColumnName("TheLoai").HasMaxLength(100).IsRequired();
        b.Property(x => x.Isbn).HasColumnName("ISBN").HasMaxLength(20).IsUnicode(false).IsRequired();
        b.Property(x => x.Description).HasColumnName("MoTa");
        b.Property(x => x.CoverImage).HasColumnName("AnhBia").HasMaxLength(255).IsUnicode(false);
        b.Property(x => x.DefaultShelf).HasColumnName("ViTriKeMacDinh").HasMaxLength(20).IsUnicode(false);
        b.Property(x => x.CreatedAt).HasColumnName("NgayTao");

        b.HasIndex(x => x.Isbn).IsUnique();
        b.HasIndex(x => x.Title);

        b.HasMany(x => x.Copies).WithOne(c => c.BookTitle).HasForeignKey(c => c.BookTitleId);
    }
}
