using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Mazad.UseCases.CategoryDomain.CategoryAttributes.Read;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Read;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Read;

public class GetAllCategoriesQuery : BaseQuery<List<CategoryDto>> { }

public class GetAllCategoriesQueryHandler
    : BaseQueryHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly MazadDbContext _context;

    public GetAllCategoriesQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<List<CategoryDto>>> Handle(GetAllCategoriesQuery query)
    {
        // 1. Get all categories from the database
        // We include ParentCategory and Children to ensure they are loaded if not already.
        // However, the current model loads ParentCategory and Children for each category.
        // For a more efficient approach, especially with many categories, it's better to fetch
        // all categories and then build the hierarchy in memory.
        var allCategories = await _context
            .Categories.AsNoTracking()
            .Include(c => c.CategoryAttributes)
            .ThenInclude(ca => ca.DynamicAttribute)
            .ToListAsync();

        // 2. Store them in a dictionary for efficient lookup by Id
        var categoryDictionary = allCategories.ToDictionary(c => c.Id);

        // 3. Build the hierarchical DTO structure
        var categoryDtos = new List<CategoryDto>();

        foreach (var category in allCategories)
        {
            // If it's a root category (no parent), start building its subtree
            if (category.ParentId == null)
            {
                categoryDtos.Add(MapCategoryToDto(category, categoryDictionary, query.Language));
            }
        }

        return Result<List<CategoryDto>>.Ok(
            categoryDtos,
            new LocalizedMessage
            {
                Arabic = "تم الحصول على جميع الفئات بنجاح",
                English = "All categories retrieved successfully",
            }
        );
    }

    /// <summary>
    /// Recursively maps a Category entity to a CategoryDto, building the children hierarchy.
    /// </summary>
    /// <param name="category">The Category entity to map.</param>
    /// <param name="categoryDictionary">A dictionary of all categories for efficient lookup.</param>
    /// <param name="language">The desired language for category names.</param>
    /// <returns>A CategoryDto with its children populated.</returns>
    private CategoryDto MapCategoryToDto(
        Category category,
        Dictionary<int, Category> categoryDictionary,
        string language
    )
    {
        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = language == "ar" ? category.NameArabic : category.NameEnglish,
            IsActive = category.IsActive,
            Children = new List<CategoryDto>(),
            DynamicAttributes = category
                .CategoryAttributes.Select(ca => new CategoryDynamicAttributeDto
                {
                    Id = ca.DynamicAttributeId,
                    Name =
                        language == "ar"
                            ? ca.DynamicAttribute.NameArabic
                            : ca.DynamicAttribute.NameEnglish,
                    IsActive = ca.DynamicAttribute.IsActive,
                    AttributeValueTypeString = DynamicAttributeHelper.RepresentAttributeValueType(
                        ca.DynamicAttribute.AttributeValueType,
                        language
                    ),
                    IsSelected = true,
                })
                .ToList(),
        };

        // Find children of the current category from the dictionary
        var children = categoryDictionary.Values.Where(c => c.ParentId == category.Id).ToList();

        foreach (var child in children)
        {
            categoryDto.Children.Add(MapCategoryToDto(child, categoryDictionary, language)); // Recursively add children
        }

        return categoryDto;
    }
}

// Ensure the CategoryDto remains the same as provided in the problem description
public class CategoryDto
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required List<CategoryDto> Children { get; set; } = [];
    public required bool IsActive { get; set; }
    public required List<CategoryDynamicAttributeDto> DynamicAttributes { get; set; } = [];
}
