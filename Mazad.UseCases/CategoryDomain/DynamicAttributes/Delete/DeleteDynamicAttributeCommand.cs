using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Delete;

public class DeleteDynamicAttributeCommand : BaseCommand
{
    public int Id { get; set; }
}

public class DeleteDynamicAttributeApiRequest : BaseApiRequest<DeleteDynamicAttributeCommand>
{
    public int Id { get; set; }

    public override DeleteDynamicAttributeCommand ToCommand(int userId, string language)
    {
        return new DeleteDynamicAttributeCommand
        {
            Id = Id,
            UserId = userId,
            Language = language
        };
    }
} 