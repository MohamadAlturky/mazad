using Mazad.Core.Shared.Entities;

namespace Mazad.Core.Domain.Categories;

public class CategoryAttribute : BaseEntity<int>
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }
    public Category Category { get; set; } = null!;
    public DynamicAttribute DynamicAttribute { get; set; } = null!;
}

