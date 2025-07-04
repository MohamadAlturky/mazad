using Mazad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
