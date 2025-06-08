using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Create;

public class CreateDynamicAttributeCommand : BaseCommand
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public AttributeValueType AttributeValueType { get; set; }
}


public class CreateDynamicAttributeApiRequest : BaseApiRequest<CreateDynamicAttributeCommand>
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public AttributeValueType AttributeValueType { get; set; }


    public override CreateDynamicAttributeCommand ToCommand(int userId, string language)
    {
        return new CreateDynamicAttributeCommand
        {
            NameArabic = NameArabic,
            NameEnglish = NameEnglish,
            AttributeValueType = AttributeValueType,
            UserId = userId,
            Language = language
        };
    }
}