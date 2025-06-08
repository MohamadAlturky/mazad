using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mazad.Core.Domain.Categories.Db;

public class DynamicAttributeConfiguration : IEntityTypeConfiguration<DynamicAttribute>
{
    public void Configure(EntityTypeBuilder<DynamicAttribute> builder)
    {
        builder.ToTable("DynamicAttributes", "dbo");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.NameArabic);
        builder.HasIndex(c => c.NameEnglish);
        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.HasMany(c => c.CategoryAttributes)
            .WithOne(e => e.DynamicAttribute)
            .HasForeignKey(c => c.DynamicAttributeId);
        builder.HasMany(c => c.CategoryAttributes)
            .WithOne(e => e.DynamicAttribute)
            .HasForeignKey(c => c.DynamicAttributeId);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
