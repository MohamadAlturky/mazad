using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Delete;

public class DeleteCategoryCommandHandler : BaseCommandHandler<DeleteCategoryCommand>
{
    private readonly MazadDbContext _context;
    private readonly DeleteCategoryCommandValidator _validator;

    public DeleteCategoryCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new DeleteCategoryCommandValidator();
    }

    public override async Task<Result> Handle(DeleteCategoryCommand command)
    {
        // 1. Validate the command
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult; // Return validation errors immediately
        }

        // 2. Find the category to delete
        var categoryToDelete = await _context.Categories.FindAsync(command.Id);
        if (categoryToDelete == null)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "الفئة غير موجودة.",
                English = "Category not found."
            });
        }

        // 3. Check for child categories
        var hasChildren = await _context.Categories.AnyAsync(c => c.ParentId == command.Id);
        if (hasChildren)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "لا يمكن حذف هذه الفئة لأن لديها فئات فرعية مرتبطة بها.",
                English = "This category cannot be deleted as it has child categories associated with it."
            });
        }

        // Optional: Check for associated products or other entities
        // If categories can have products, you'd add a check here:
        // var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == command.Id);
        // if (hasProducts) { /* return error */ }

        // 4. Remove the category from the database context
        categoryToDelete.IsDeleted = true;
        categoryToDelete.DeletedAt = DateTime.UtcNow;
        _context.Categories.Update(categoryToDelete);

        // 5. Save changes to the database
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the exception (e.g., using a logging framework)
            Console.WriteLine($"Error deleting category: {ex.Message}");
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حذف الفئة.",
                English = "An error occurred while deleting the category."
            }, ex);
        }

        // 6. Return a success result
        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم حذف الفئة بنجاح.",
            English = "Category deleted successfully."
        });
    }
}