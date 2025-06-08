using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Create;

public class CreateCategoryAttributeCommand : BaseCommand
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }
}

public class CreateCategoryAttributeApiRequest : BaseApiRequest<CreateCategoryAttributeCommand>
{
    public int CategoryId { get; set; }
    public int DynamicAttributeId { get; set; }

    public override CreateCategoryAttributeCommand ToCommand(int userId, string language)
    {
        return new CreateCategoryAttributeCommand
        {
            CategoryId = CategoryId,
            DynamicAttributeId = DynamicAttributeId,
            UserId = userId,
            Language = language
        };
    }
}