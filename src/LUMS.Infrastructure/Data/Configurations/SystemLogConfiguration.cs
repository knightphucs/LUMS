using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    public void Configure(EntityTypeBuilder<SystemLog> b)
    {
        b.ToTable("NHAT_KY_HE_THONG");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaNhatKy");
        b.Property(x => x.AccountId).HasColumnName("MaTaiKhoan");
        b.Property(x => x.Action).HasColumnName("HanhDong").HasMaxLength(255).IsRequired();
        b.Property(x => x.RelatedObject).HasColumnName("DoiTuongLienQuan").HasMaxLength(100);
        b.Property(x => x.Timestamp).HasColumnName("ThoiGian");

        b.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId);
        b.HasIndex(x => x.Timestamp);
    }
}
