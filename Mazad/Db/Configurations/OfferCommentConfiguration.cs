using Mazad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Users.Db;

public class OfferCommentConfiguration : IEntityTypeConfiguration<OfferComment>
{
    public void Configure(EntityTypeBuilder<OfferComment> builder)
    {
        builder.ToTable("OfferComments", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder
            .HasOne(o => o.Offer)
            .WithMany(o => o.Comments)
            .HasForeignKey(o => o.OfferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(o => o.ReplyToComment)
            .WithMany(o => o.ChildrenComments)
            .HasForeignKey(o => o.ReplyToCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(o => o.ChildrenComments)
            .WithOne(o => o.ReplyToComment)
            .HasForeignKey(o => o.ReplyToCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
