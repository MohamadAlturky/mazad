using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Categories.Read;

public class GetCategoriesTreeByOneQuery : BaseQuery<List<CategoryDto>>
{
    public int CategoryId { get; set; }
}

public class GetCategoriesTreeByOneQueryHandler : BaseQueryHandler<GetCategoriesTreeByOneQuery, List<CategoryDto>>
{
    private readonly MazadDbContext _context;

    public GetCategoriesTreeByOneQueryHandler(MazadDbContext context)
    {
        _context = context;
    }
public override async Task<Result<List<CategoryDto>>> Handle(GetCategoriesTreeByOneQuery query)
{
    // Step 1: Load all categories from DB in one query
    var allCategories = await _context.Categories
        .AsNoTracking()
        .ToListAsync();

    // Step 2: Build a dictionary for quick access
    var categoryDict = allCategories.ToDictionary(c => c.Id);

    // Step 3: Build tree structure in-memory
    var root = categoryDict.GetValueOrDefault(query.CategoryId);
    if (root == null)
    {
        return Result<List<CategoryDto>>.Fail(new LocalizedMessage
        {
            Arabic = "الصنف غير موجود",
            English = "Category not found"
        });
    }

    // Step 4: Map categories to DTOs and build the hierarchy
    var dtoDict = new Dictionary<int, CategoryDto>();
    foreach (var category in allCategories)
    {
        dtoDict[category.Id] = new CategoryDto
        {
            Id = category.Id,
            Name = query.Language == "ar" ? category.NameArabic : category.NameEnglish,
            IsActive = category.IsActive,
            Children = []
        };
    }

    // Step 5: Populate the tree by linking children to their parents
    foreach (var category in allCategories)
    {
        if (category.ParentId.HasValue && dtoDict.ContainsKey(category.ParentId.Value))
        {
            dtoDict[category.ParentId.Value].Children.Add(dtoDict[category.Id]);
        }
    }

    var tree = new List<CategoryDto> { dtoDict[root.Id] };

    return Result<List<CategoryDto>>.Ok(tree, new LocalizedMessage
    {
        Arabic = "تم الحصول على الصنف بنجاح",
        English = "Category retrieved successfully"
    });
}


}

//  public override async Task<Result<List<CategoryDto>>> Handle(GetCategoriesTreeByOneQuery query)
//     {
//         var rootCategory = await _context.Categories
//             .Where(c => c.Id == query.CategoryId)
//             .Include(c => c.Children)
//             .FirstOrDefaultAsync();

//         if (rootCategory is null)
//         {
//             return Result<List<CategoryDto>>.Fail(new LocalizedMessage
//             {
//                 Arabic = "الصنف غير موجود",
//                 English = "Category not found"
//             });
//         }

//         var categoryTree = new List<CategoryDto>();
//         await PopulateCategoryTree(rootCategory, categoryTree, query.Language);

//         return Result<List<CategoryDto>>.Ok(categoryTree, new LocalizedMessage
//         {
//             Arabic = "تم الحصول على الصنف بنجاح",
//             English = "Category retrieved successfully"
//         });
//     }

//     private async Task PopulateCategoryTree(Category category, List<CategoryDto> categoryList, string language)
//     {
//         var categoryDto = new CategoryDto
//         {
//             Id = category.Id,
//             Name = language == "ar" ? category.NameArabic : category.NameEnglish,
//             IsActive = category.IsActive,
//             Children = []
//         };

//         categoryList.Add(categoryDto);

//         // Explicitly load children for the current category if not already loaded
//         // This is important for recursive loading beyond the first level
//         if (category.Children.Count == 0)
//         {
//             await _context.Entry(category)
//                 .Collection(c => c.Children)
//                 .LoadAsync();
//         }
//         foreach (var child in category.Children.OrderBy(c => c.Id)) // Order children for consistent results
//         {
//             await PopulateCategoryTree(child, categoryDto.Children, language);
//         }
//     }