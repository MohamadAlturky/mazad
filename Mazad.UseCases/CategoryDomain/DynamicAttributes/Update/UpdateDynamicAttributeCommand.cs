using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Update;

public class UpdateDynamicAttributeCommand : BaseCommand
{
    public int Id { get; set; }
    public string? NameArabic { get; set; }
    public string? NameEnglish { get; set; }
    public AttributeValueType? AttributeValueType { get; set; }
}

public class UpdateDynamicAttributeApiRequest : BaseApiRequest<UpdateDynamicAttributeCommand>
{
    public int Id { get; set; }
    public string? NameArabic { get; set; }
    public string? NameEnglish { get; set; }
    public AttributeValueType? AttributeValueType { get; set; }
    public override UpdateDynamicAttributeCommand ToCommand(int userId, string language)
    {
        return new UpdateDynamicAttributeCommand
        {
            Id = Id,
            NameArabic = NameArabic,
            NameEnglish = NameEnglish,
            AttributeValueType = AttributeValueType,
            UserId = userId,
            Language = language
        };
    }
}