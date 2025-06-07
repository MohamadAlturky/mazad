using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Read;

public class GetCategoriesListQuery : BaseQuery<GetCategoriesListQueryResponse>
{
    public bool? FilterByIsActiveEquals { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}

public class GetCategoriesListQueryHandler : BaseQueryHandler<GetCategoriesListQuery, GetCategoriesListQueryResponse>
{
    private readonly MazadDbContext _context;

    public GetCategoriesListQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<GetCategoriesListQueryResponse>> Handle(GetCategoriesListQuery query)
    {
        var queryable = _context.Categories.Where(c => c.ParentId == null).AsNoTracking();
        if (query.FilterByIsActiveEquals.HasValue)
        {
            queryable = queryable.Where(c => c.IsActive == query.FilterByIsActiveEquals.Value);
        }
        var allCategories = await queryable
        .Skip((query.PageNumber - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToListAsync();

        var totalCount = await queryable.CountAsync();

        var categoryDtos = new List<CategoryListDto>();
        foreach (var category in allCategories)
        {
            if (category.ParentId == null)
            {
                categoryDtos.Add(MapCategoryToDto(category, query.Language));
            }
        }

        return Result<GetCategoriesListQueryResponse>.Ok(new GetCategoriesListQueryResponse
        {
            Categories = categoryDtos,
            TotalCount = totalCount
        }, new LocalizedMessage
        {
            Arabic = "تم الحصول على جميع الفئات بنجاح",
            English = "All categories retrieved successfully"
        });
    }

    private CategoryListDto MapCategoryToDto(Category category, string language)
    {
        var categoryDto = new CategoryListDto
        {
            Id = category.Id,
            Name = language == "ar" ? category.NameArabic : category.NameEnglish,
            IsActive = category.IsActive
        };
        return categoryDto;
    }
}

public class CategoryListDto
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
}

public class GetCategoriesListQueryResponse
{
    public required List<CategoryListDto> Categories { get; set; }
    public required int TotalCount { get; set; }
}


