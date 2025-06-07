using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Read;

public class GetCategoriesDropdownQuery : BaseQuery<List<CategoryDropdownDto>>
{
}

public class GetCategoriesDropdownQueryHandler : BaseQueryHandler<GetCategoriesDropdownQuery, List<CategoryDropdownDto>>
{
    private readonly MazadDbContext _context;

    public GetCategoriesDropdownQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<List<CategoryDropdownDto>>> Handle(GetCategoriesDropdownQuery query)
    {
        var allCategories = await _context.Categories.AsNoTracking().ToListAsync();

        var categoryDictionary = allCategories.ToDictionary(c => c.Id);

        var categoryDtos = new List<CategoryDropdownDto>();
        foreach (var category in allCategories)
        {
            Category? parent = categoryDictionary.TryGetValue(category.ParentId ?? 0, out var parentCategory) ? parentCategory : null;
            string parentName = string.Empty;
            if (parent is not null)
            {
                parentName = query.Language == "ar" ? parent.NameArabic : parent.NameEnglish;
            }
            categoryDtos.Add(new CategoryDropdownDto
            {
                Id = category.Id,
                Name = query.Language == "ar" ? category.NameArabic : category.NameEnglish,
                ParentName = parentName,
                IsActive = category.IsActive
            });
        }

        return Result<List<CategoryDropdownDto>>.Ok(categoryDtos, new LocalizedMessage
        {
            Arabic = "تم الحصول على جميع الفئات بنجاح",
            English = "All categories retrieved successfully"
        });
    }
}

// Ensure the CategoryDto remains the same as provided in the problem description
public class CategoryDropdownDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
}