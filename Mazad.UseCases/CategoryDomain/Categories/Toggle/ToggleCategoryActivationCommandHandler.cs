using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Categories.Toggle;

public class ToggleCategoryActivationCommandHandler : BaseCommandHandler<ToggleCategoryActivationCommand>
{
    private readonly MazadDbContext _context;

    public ToggleCategoryActivationCommandHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result> Handle(ToggleCategoryActivationCommand command)
    {
        var category = await _context.Categories.FindAsync(command.CategoryId);
        if (category == null)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "التصنيف غير موجود",
                English = "Category not found"
            });
        }
        category.IsActive = !category.IsActive;
        await _context.SaveChangesAsync();
        string arabic = "";
        string english = "";
        if (category.IsActive)
        {
            arabic = "تم تفعيل الفئة بنجاح";
            english = "the category has been activated succesfully";
        }
        else
        {
            arabic = "تم إالغاء تفعيل الفئة بنجاح";
            english = "the category has been deactivated succesfully";
        }
        return Result.Ok(
                new LocalizedMessage
                {
                    Arabic = arabic,
                    English = english
                });
    }
}