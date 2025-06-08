using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Create;

public class CreateCategoryAttributeCommandHandler : BaseCommandHandler<CreateCategoryAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly CreateCategoryAttributeCommandValidator _validator;

    public CreateCategoryAttributeCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new CreateCategoryAttributeCommandValidator();
    }

    public override async Task<Result> Handle(CreateCategoryAttributeCommand command)
    {
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        // Check if the category exists
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == command.CategoryId);
        if (!categoryExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "الفئة غير موجودة.",
                English = "Category does not exist."
            });
        }

        // Check if the dynamic attribute exists
        var attributeExists = await _context.DynamicAttributes.AnyAsync(a => a.Id == command.DynamicAttributeId);
        if (!attributeExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "السمة غير موجودة.",
                English = "Dynamic attribute does not exist."
            });
        }

        // Check if the category-attribute relationship already exists
        var relationshipExists = await _context.CategoryAttributes
            .AnyAsync(ca => ca.CategoryId == command.CategoryId && ca.DynamicAttributeId == command.DynamicAttributeId);

        if (relationshipExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "هذه السمة موجودة بالفعل في هذه الفئة.",
                English = "This attribute already exists in this category."
            });
        }

        var newCategoryAttribute = new CategoryAttribute
        {
            CategoryId = command.CategoryId,
            DynamicAttributeId = command.DynamicAttributeId,
            IsActive = true,
            IsDeleted = false,
        };

        _context.CategoryAttributes.Add(newCategoryAttribute);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حفظ العلاقة بين الفئة والسمة.",
                English = "An error occurred while saving the category-attribute relationship."
            }, ex);
        }

        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم إضافة السمة إلى الفئة بنجاح.",
            English = "Attribute added to category successfully."
        });
    }
}