using LUMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LUMS.Infrastructure.Data.Configurations;

public class PolicyConfigurationConfiguration : IEntityTypeConfiguration<PolicyConfiguration>
{
    public void Configure(EntityTypeBuilder<PolicyConfiguration> b)
    {
        b.ToTable("CAU_HINH_QUY_DINH");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("MaCauHinh");
        b.Property(x => x.MaxLoanDays).HasColumnName("SoNgayMuonToiDa");
        b.Property(x => x.MaxBooksPerReader).HasColumnName("SoSachMuonToiDa");
        b.Property(x => x.MaxReservations).HasColumnName("SoDatTruocToiDa");
        b.Property(x => x.FinePerDay).HasColumnName("MucPhatMoiNgay").HasColumnType("decimal(12,0)");
        b.Property(x => x.UpdatedById).HasColumnName("NguoiCapNhat");
        b.Property(x => x.EffectiveFrom).HasColumnName("NgayApDung");

        b.HasOne(x => x.UpdatedBy).WithMany().HasForeignKey(x => x.UpdatedById);
    }
}
