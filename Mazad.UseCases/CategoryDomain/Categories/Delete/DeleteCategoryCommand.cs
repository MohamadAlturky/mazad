using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Categories.Delete;

public class DeleteCategoryCommand : BaseCommand
{
    public int Id { get; set; }
}

public class DeleteCategoryApiRequest : BaseApiRequest<DeleteCategoryCommand>
{
    public int Id { get; set; }

    public override DeleteCategoryCommand ToCommand(int userId, string language)
    {
        return new DeleteCategoryCommand
        {
            Id = Id,
            UserId = userId,
            Language = language
        };
    }
}