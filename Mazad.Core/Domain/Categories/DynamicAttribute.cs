using Mazad.Core.Shared.Entities;

namespace Mazad.Core.Domain.Categories;

public class DynamicAttribute : BaseEntity<int>
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public ICollection<CategoryAttributes> CategoryAttributes { get; set; } = [];
    public AttributeValueType AttributeValueType { get; set; }

}
public enum AttributeValueType
{
    String = 1,
    Number = 2,
    Boolean = 3,
}