using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Update;

public class UpdateDynamicAttributeCommandHandler : BaseCommandHandler<UpdateDynamicAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly UpdateDynamicAttributeCommandValidator _validator;

    public UpdateDynamicAttributeCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new UpdateDynamicAttributeCommandValidator();
    }

    public override async Task<Result> Handle(UpdateDynamicAttributeCommand command)
    {
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        var attribute = await _context.DynamicAttributes
            .FirstOrDefaultAsync(a => a.Id == command.Id);

        if (attribute == null)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "السمة غير موجودة.",
                English = "Attribute not found."
            });
        }

        var attributeExists = await _context.DynamicAttributes
            .AnyAsync(a => (a.NameArabic == command.NameArabic || a.NameEnglish == command.NameEnglish) && a.Id != command.Id);

        if (attributeExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "سمة بنفس الاسم موجودة بالفعل.",
                English = "An attribute with the same name already exists."
            });
        }

        attribute.NameArabic = command.NameArabic ?? attribute.NameArabic;
        attribute.NameEnglish = command.NameEnglish ?? attribute.NameEnglish;
        attribute.AttributeValueType = command.AttributeValueType ?? attribute.AttributeValueType;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء تحديث السمة.",
                English = "An error occurred while updating the attribute."
            }, ex);
        }

        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم تحديث السمة بنجاح.",
            English = "Attribute updated successfully."
        });
    }
}
