using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Toggle;

public class ToggleCategoryAttributeCommand : BaseCommand
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }
}

public class ToggleCategoryAttributeApiRequest : BaseApiRequest<ToggleCategoryAttributeCommand>
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }

    public override ToggleCategoryAttributeCommand ToCommand(int userId, string language)
    {
        return new ToggleCategoryAttributeCommand
        {
            CategoryId = CategoryId,
            DynamicAttributeId = DynamicAttributeId,
            UserId = userId,
            Language = language
        };
    }
}