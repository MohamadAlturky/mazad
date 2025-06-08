using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Toggle;

public class ToggleDynamicAttributeCommand : BaseCommand
{
    public int Id { get; set; }
}

public class ToggleDynamicAttributeApiRequest : BaseApiRequest<ToggleDynamicAttributeCommand>
{
    public int Id { get; set; }

    public override ToggleDynamicAttributeCommand ToCommand(int userId, string language)
    {
        return new ToggleDynamicAttributeCommand
        {
            Id = Id,
            UserId = userId,
            Language = language
        };
    }
}