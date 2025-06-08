using Mazad.Core.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Categories.Db;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name and schema (optional, EF Core uses DbSet name by default)
        builder.ToTable("Categories", "dbo");

        // Primary Key (assuming BaseEntity<int> handles 'Id' as PK)
        // If 'Id' is the primary key and inherited from BaseEntity, EF Core will discover it by convention.
        // If it's named differently or you want to explicitly configure, you would do:
        builder.HasKey(c => c.Id);

        // Property configurations
        builder.Property(c => c.NameArabic)
            .IsRequired() // NameArabic is required
            .HasMaxLength(255); // Example max length

        builder.Property(c => c.NameEnglish)
            .IsRequired() // NameEnglish is required
            .HasMaxLength(255); // Example max length

        // Self-referencing relationship for ParentCategory
        builder.HasOne(c => c.ParentCategory) // Category has one ParentCategory
            .WithMany(e => e.Children) // ParentCategory can have many child categories (no navigation property on parent side)
            .HasForeignKey(c => c.ParentId) // Foreign key is ParentId
            .IsRequired(false) // ParentId is nullable
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete (e.g., if a parent category is deleted, its children are not automatically deleted)

        // You can also add unique constraints, indexes, etc.
        // Example: Adding an index to NameArabic for faster lookups
        builder.HasIndex(c => c.NameArabic);
        builder.HasIndex(c => c.NameEnglish);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
