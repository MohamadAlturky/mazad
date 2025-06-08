using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Delete;

public class DeleteDynamicAttributeCommandHandler : BaseCommandHandler<DeleteDynamicAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly DeleteDynamicAttributeCommandValidator _validator;

    public DeleteDynamicAttributeCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new DeleteDynamicAttributeCommandValidator();
    }

    public override async Task<Result> Handle(DeleteDynamicAttributeCommand command)
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
        attribute.IsDeleted = true;
        attribute.DeletedAt = DateTime.UtcNow;
        _context.DynamicAttributes.Update(attribute);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حذف السمة.",
                English = "An error occurred while deleting the attribute."
            }, ex);
        }

        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم حذف السمة بنجاح.",
            English = "Attribute deleted successfully."
        });
    }
}