using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Update;

public class UpdateCategoryCommandHandler : BaseCommandHandler<UpdateCategoryCommand>
{
    private readonly MazadDbContext _context;
    private readonly UpdateCategoryCommandValidator _validator;

    public UpdateCategoryCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new UpdateCategoryCommandValidator();
    }

    public override async Task<Result> Handle(UpdateCategoryCommand command)
    {
        // 1. Validate the command
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult; // Return validation errors immediately
        }

        // 2. Retrieve the category to update
        var categoryToUpdate = await _context.Categories.FindAsync(command.Id);
        if (categoryToUpdate is null)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "الفئة غير موجودة.",
                English = "Category not found."
            });
        }

        // 3. Check for duplicate category names (excluding the current category)
        var categoryExists = await _context.Categories
            .AnyAsync(c => (c.NameArabic == command.NameArabic || c.NameEnglish == command.NameEnglish) && c.Id != command.Id);

        if (categoryExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "فئة بنفس الاسم العربي أو الإنجليزي موجودة بالفعل.",
                English = "A category with the same Arabic or English name already exists."
            });
        }

        // 4. Validate ParentId if provided
        if (command.ParentId.HasValue)
        {
            if (command.ParentId.Value == command.Id)
            {
                return Result.Fail(new LocalizedMessage
                {
                    Arabic = "لا يمكن أن تكون الفئة الأب هي نفسها الفئة.",
                    English = "Parent category cannot be the category itself."
                });
            }

            var parentCategory = await _context.Categories.FirstOrDefaultAsync(e => e.Id == command.ParentId.Value);
            if (parentCategory is null)
            {
                return Result.Fail(new LocalizedMessage
                {
                    Arabic = "الفئة الأب غير موجودة.",
                    English = "The parent category does not exist."
                });
            }

            // Optional: Prevent circular dependencies (e.g., A -> B -> A)
            // This would require a more complex check, potentially traversing the hierarchy.
            // For simplicity, it's omitted here but good to consider for real-world applications.
        }

        // 5. Update category properties
        categoryToUpdate.NameArabic = command.NameArabic ?? categoryToUpdate.NameArabic;
        categoryToUpdate.NameEnglish = command.NameEnglish ?? categoryToUpdate.NameEnglish;
        categoryToUpdate.ParentId = command.ParentId;

        // 6. Save changes to the database
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the exception
            Console.WriteLine($"Error updating category: {ex.Message}");
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حفظ الفئة.",
                English = "An error occurred while saving the category."
            }, ex);
        }

        // 7. Return a success result
        return Result<Category>.Ok(categoryToUpdate, new LocalizedMessage
        {
            Arabic = "تم تحديث الفئة بنجاح.",
            English = "Category updated successfully."
        });
    }
}