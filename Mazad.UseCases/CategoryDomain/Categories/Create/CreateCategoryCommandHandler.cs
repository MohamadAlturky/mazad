using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Create;

public class CreateCategoryCommandHandler : BaseCommandHandler<CreateCategoryCommand>
{
    private readonly MazadDbContext _context;
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandHandler(MazadDbContext context)
    {
        _context = context;
        _validator = new CreateCategoryCommandValidator();
    }

    public override async Task<Result> Handle(CreateCategoryCommand command)
    {
        // 1. Validate the command
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult; // Return validation errors immediately
        }

        // 2. Check for duplicate category names (optional, but good practice)
        // You might want to enforce uniqueness for categories under the same parent,
        // or globally, depending on your business rules.
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.NameArabic == command.NameArabic || c.NameEnglish == command.NameEnglish);

        if (categoryExists)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "فئة بنفس الاسم العربي أو الإنجليزي موجودة بالفعل.",
                English = "A category with the same Arabic or English name already exists."
            });
        }
        if (command.ParentId.HasValue)
        {
            var parentCategory = _context.Categories.FirstOrDefault(e=>e.Id == command.ParentId.Value);
            if (parentCategory is null)
            {
                return Result.Fail(new LocalizedMessage
                {
                    Arabic = "الفئة الأب غير موجودة.",
                    English = "The parent category does not exist."
                });
            }
        }

        // 3. Create a new Category entity
        var newCategory = new Category
        {
            NameArabic = command.NameArabic,
            NameEnglish = command.NameEnglish,
            ParentId = command.ParentId,
            IsActive = true,
        };

        // 4. Add the new category to the database context
        _context.Categories.Add(newCategory);

        // 5. Save changes to the database
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the exception (e.g., using a logging framework)
            Console.WriteLine($"Error saving category: {ex.Message}");
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "حدث خطأ أثناء حفظ الفئة.",
                English = "An error occurred while saving the category."
            }, ex);
        }

        // 6. Return a success result
        return Result<Category>.Ok(newCategory, new LocalizedMessage
        {
            Arabic = "تم إنشاء الفئة بنجاح.",
            English = "Category created successfully."
        });
    }
}