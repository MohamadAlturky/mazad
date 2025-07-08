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
        
        // Make PhoneNumber unique only when it has a value
        builder.HasIndex(e => e.PhoneNumber)
            .HasDatabaseName("IX_Users_PhoneNumber")
            .IsUnique()
            .HasFilter("[PhoneNumber] IS NOT NULL");  // SQL Server specific - ignore nulls in unique index
            
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Users_Name");

        // Make ProfilePhotoUrl optional
        builder.Property(e => e.ProfilePhotoUrl)
            .IsRequired(false);

        // Make PhoneNumber optional
        builder.Property(e => e.PhoneNumber)
            .IsRequired(false);
    }
}
