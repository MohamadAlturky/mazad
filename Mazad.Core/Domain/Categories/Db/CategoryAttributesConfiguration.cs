using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Categories.Db;

public class CategoryAttributesConfiguration : IEntityTypeConfiguration<CategoryAttributes>
{
    public void Configure(EntityTypeBuilder<CategoryAttributes> builder)
    {
        builder.ToTable("CategoryAttributes", "dbo");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.CategoryId);
        builder.HasIndex(c => c.DynamicAttributeId);
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
