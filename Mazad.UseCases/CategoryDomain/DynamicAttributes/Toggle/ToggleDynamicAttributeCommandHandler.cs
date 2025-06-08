using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Toggle;

public class ToggleDynamicAttributeCommandHandler : BaseCommandHandler<ToggleDynamicAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly ToggleDynamicAttributeCommandValidator _validator;

    public ToggleDynamicAttributeCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new ToggleDynamicAttributeCommandValidator();
    }

    public override async Task<Result> Handle(ToggleDynamicAttributeCommand command)
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

        // Toggle the IsActive status
        attribute.IsActive = !attribute.IsActive;
        _context.DynamicAttributes.Update(attribute);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء تغيير حالة السمة.",
                English = "An error occurred while toggling the attribute status."
            }, ex);
        }

        return Result.Ok(new LocalizedMessage
        {
            Arabic = attribute.IsActive
                ? "تم تفعيل السمة بنجاح."
                : "تم تعطيل السمة بنجاح.",
            English = attribute.IsActive
                ? "Attribute has been activated successfully."
                : "Attribute has been deactivated successfully."
        });
    }
}