using Mazad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Users.Db;

public class FollowerConfiguration : IEntityTypeConfiguration<Follower>
{
    public void Configure(EntityTypeBuilder<Follower> builder)
    {
        builder.ToTable("Followers", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Create a unique index on FollowerId and FollowedId combination
        builder
            .HasIndex(f => new { f.FollowerId, f.FollowedId })
            .HasDatabaseName("IX_Followers_FollowerId_FollowedId")
            .IsUnique();

        builder
            .HasOne(f => f.TheFollower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(f => f.TheFollowed)
            .WithMany()
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
