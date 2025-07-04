using Mazad.Core.Domain.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Users.Db;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("Regions", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder
            .HasOne(r => r.ParentRegion)
            .WithMany(r => r.SubRegions)
            .HasForeignKey(r => r.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
