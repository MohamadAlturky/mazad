using Mazad.Core.Shared.Entities;

namespace Mazad.Core.Domain.Categories;

public class Category : BaseEntity<int>
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;

    public int? ParentId { get; set; }
    public Category? ParentCategory { get; set; } = null;
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<CategoryAttribute> CategoryAttributes { get; set; } = [];
}
