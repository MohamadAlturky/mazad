using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Create;

public class CreateDynamicAttributeCommandHandler : BaseCommandHandler<CreateDynamicAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly CreateDynamicAttributeCommandValidator _validator;

    public CreateDynamicAttributeCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new CreateDynamicAttributeCommandValidator();
    }

    public override async Task<Result> Handle(CreateDynamicAttributeCommand command)
    {
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        var attributeExists = await _context.DynamicAttributes
            .AnyAsync(a => a.NameArabic == command.NameArabic || a.NameEnglish == command.NameEnglish);

        if (attributeExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "سمة بنفس الاسم موجودة بالفعل في هذه الفئة.",
                English = "An attribute with the same name already exists in this category."
            });
        }

        var newAttribute = new DynamicAttribute
        {
            NameArabic = command.NameArabic,
            NameEnglish = command.NameEnglish,
            IsActive = true,
            AttributeValueType = command.AttributeValueType
        };

        _context.DynamicAttributes.Add(newAttribute);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حفظ السمة.",
                English = "An error occurred while saving the attribute."
            }, ex);
        }

        // 7. Return a success result
        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم إنشاء السمة بنجاح.",
            English = "Attribute created successfully."
        });
    }
}