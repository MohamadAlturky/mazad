using Mazad.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Users.Db;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo");

        builder.HasKey(c => c.Id);
        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.HasIndex(e => e.PhoneNumber).HasDatabaseName("IX_Users_PhoneNumber").IsUnique();
        builder.HasIndex(e => e.Name).HasDatabaseName("IX_Users_Name");
    }
}
