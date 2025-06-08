using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Delete;

public class DeleteCategoryAttributeCommand : BaseCommand
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }
}

public class DeleteCategoryAttributeApiRequest : BaseApiRequest<DeleteCategoryAttributeCommand>
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }

    public override DeleteCategoryAttributeCommand ToCommand(int userId, string language)
    {
        return new DeleteCategoryAttributeCommand
        {
            CategoryId = CategoryId,
            DynamicAttributeId = DynamicAttributeId,
            UserId = userId,
            Language = language
        };
    }
} 