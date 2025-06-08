using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Categories.Db;

public class CategoryAttributesConfiguration : IEntityTypeConfiguration<CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        builder.ToTable("CategoryAttributes", "dbo");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.CategoryId);
        builder.HasIndex(c => c.DynamicAttributeId);
        // Add unique index for the combination of CategoryId and DynamicAttributeId
        builder.HasIndex(c => new { c.CategoryId, c.DynamicAttributeId })
            .IsUnique();
        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.HasOne(c => c.Category)
            .WithMany(e => e.CategoryAttributes)
            .HasForeignKey(c => c.CategoryId);

        builder.HasOne(c => c.DynamicAttribute)
            .WithMany(e => e.CategoryAttributes)
            .HasForeignKey(c => c.DynamicAttributeId);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
