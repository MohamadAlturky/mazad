using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Categories.Toggle;

public class ToggleCategoryActivationCommand : BaseCommand
{
    public int CategoryId { get; set; }
}