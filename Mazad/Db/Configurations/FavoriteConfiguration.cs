using Mazad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Users.Db;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Create a unique index on UserId and ProductId combination
        builder
            .HasIndex(f => new { f.UserId, f.OfferId })
            .HasDatabaseName("IX_Favorites_UserId_OfferId")
            .IsUnique();

        builder.HasOne(f => f.Offer).WithMany().HasForeignKey(f => f.OfferId);

        builder.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId);
    }
}
