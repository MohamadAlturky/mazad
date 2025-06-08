using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Read;

public class GetCategoryBasicInfoQuery : BaseQuery<CategoryBasicInfoDto>
{
    public required int Id { get; set; }
}

public class GetCategoryBasicInfoQueryHandler : BaseQueryHandler<GetCategoryBasicInfoQuery, CategoryBasicInfoDto>
{
    private readonly MazadDbContext _context;

    public GetCategoryBasicInfoQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<CategoryBasicInfoDto>> Handle(GetCategoryBasicInfoQuery query)
    {
        var category = await _context.Categories
            .Where(c => c.Id == query.Id)
            .AsNoTracking()
            .Select(c => new CategoryBasicInfoDto
            {
                Id = c.Id,
                NameEnglish = c.NameEnglish,
                NameArabic = c.NameArabic,
                IsActive = c.IsActive,
                ParentId = c.ParentId
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return Result<CategoryBasicInfoDto>.Fail(new LocalizedMessage
            {
                Arabic = "الصنف غير موجود",
                English = "Category not found"
            });
        }

        return Result<CategoryBasicInfoDto>.Ok(category, new LocalizedMessage
        {
            Arabic = "تم الحصول على معلومات الصنف بنجاح",
            English = "Category information retrieved successfully"
        });
    }
}

public class CategoryBasicInfoDto
{
    public required int Id { get; set; }
    public required string NameEnglish { get; set; }
    public required string NameArabic { get; set; }
    public required bool IsActive { get; set; }
    public int? ParentId { get; set; }
} 