using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Delete;

public class DeleteCategoryAttributeCommandHandler : BaseCommandHandler<DeleteCategoryAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly DeleteCategoryAttributeCommandValidator _validator;

    public DeleteCategoryAttributeCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new DeleteCategoryAttributeCommandValidator();
    }

    public override async Task<Result> Handle(DeleteCategoryAttributeCommand command)
    {
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        // Find the category-attribute relationship
        var categoryAttribute = await _context.CategoryAttributes
            .FirstOrDefaultAsync(ca =>
                ca.CategoryId == command.CategoryId &&
                ca.DynamicAttributeId == command.DynamicAttributeId);

        if (categoryAttribute == null)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "العلاقة بين الفئة والسمة غير موجودة.",
                English = "The category-attribute relationship does not exist."
            });
        }

        _context.CategoryAttributes.Remove(categoryAttribute);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حذف العلاقة بين الفئة والسمة.",
                English = "An error occurred while deleting the category-attribute relationship."
            }, ex);
        }

        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم حذف السمة من الفئة بنجاح.",
            English = "Attribute removed from category successfully."
        });
    }
}