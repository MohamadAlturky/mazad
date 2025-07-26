using Mazad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mazad.Core.Domain.Regions;

namespace Mazad.Core.Domain.Users.Db;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder
            .HasMany(p => p.ImagesUrl)
            .WithOne(o => o.Offer)
            .HasForeignKey(pi => pi.OfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.Region)
            .WithMany()
            .HasForeignKey(o => o.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(o => o.Provider)
            .WithMany()
            .HasForeignKey(o => o.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OfferImageConfiguration : IEntityTypeConfiguration<OfferImage>
{
    public void Configure(EntityTypeBuilder<OfferImage> builder)
    {
        builder.ToTable("OfferImages", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
