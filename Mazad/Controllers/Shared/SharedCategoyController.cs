using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mazad.Api.Controllers;
using Mazad.Core.Domain.Users.Authentication;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/shared/categories")]
public class SharedCategoyController : BaseController
{
    private readonly MazadDbContext _context;

    public SharedCategoyController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetCategoryTree([FromQuery] GetCategoryTreeDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Build base query for categories
            var query = _context
                .Categories.Include(c => c.SubCategories.Where(sc => !sc.IsDeleted && sc.IsActive))
                .ThenInclude(sc => sc.SubCategories.Where(ssc => !ssc.IsDeleted && ssc.IsActive))
                .Where(c => !c.IsDeleted && c.IsActive);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();

                // Get IDs of all categories (including subcategories) that match the search term
                var matchingCategoryIds = await _context
                    .Categories.Where(c =>
                        !c.IsDeleted
                        && c.IsActive
                        && (
                            (isArabic ? c.NameAr : c.NameEn).ToLower().Contains(searchTerm)
                        // || (isArabic ? c.DescriptionAr : c.DescriptionEn)
                        //     .ToLower()
                        //     .Contains(searchTerm)
                        )
                    )
                    .Select(c => c.Id)
                    .ToListAsync();

                // Get IDs of parent categories that have matching subcategories
                var parentCategoryIds = await _context
                    .Categories.Where(c =>
                        !c.IsDeleted
                        && c.IsActive
                        && c.SubCategories.Any(sc =>
                            !sc.IsDeleted && sc.IsActive && matchingCategoryIds.Contains(sc.Id)
                        )
                    )
                    .Select(c => c.Id)
                    .ToListAsync();

                // Combine all matching IDs
                var allRelevantIds = matchingCategoryIds.Union(parentCategoryIds).ToList();

                // Filter root categories that either match the search term or have matching subcategories
                query = query.Where(c => c.ParentId == null && allRelevantIds.Contains(c.Id));
            }
            else
            {
                // If no search term, only get root categories
                query = query.Where(c => c.ParentId == null);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Get paginated root categories with their subcategories
            var rootCategories = await query
                .OrderBy(c => isArabic ? c.NameAr : c.NameEn)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Map categories to tree structure
            var categoryTree = rootCategories
                .Select(c => MapToCategoryTreeDto(c, isArabic))
                .ToList();

            var result = new PaginatedResult<CategoryTreeDto>
            {
                Items = categoryTree,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب شجرة الفئات بنجاح",
                    English = "Category tree retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب شجرة الفئات",
                    English = "Failed to retrieve category tree",
                },
                ex
            );
        }
    }

    [HttpGet("{categoryId}/subcategories")]
    public async Task<IActionResult> GetSubCategories(
        int categoryId,
        [FromQuery] GetSubCategoriesDto request
    )
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Check if the parent category exists and is active
            var parentCategory = await _context.Categories.FirstOrDefaultAsync(c =>
                c.Id == categoryId && !c.IsDeleted && c.IsActive
            );

            if (parentCategory == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الفئة غير موجودة أو غير نشطة",
                        English = "Category not found or inactive",
                    }
                );
            }

            // Build query for subcategories with their child count
            var query = _context
                .Categories.Include(c => c.SubCategories.Where(sc => !sc.IsDeleted && sc.IsActive))
                .Where(c => c.ParentId == categoryId && !c.IsDeleted && c.IsActive);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(c =>
                    (isArabic ? c.NameAr : c.NameEn).ToLower().Contains(searchTerm)
                // ||
                // (isArabic ? c.DescriptionAr : c.DescriptionEn).ToLower().Contains(searchTerm)
                );
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Get paginated subcategories
            var subCategories = await query
                .OrderBy(c => isArabic ? c.NameAr : c.NameEn)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CategoryListItemDto
                {
                    Id = c.Id,
                    Name = isArabic ? c.NameAr : c.NameEn,
                    // Description = isArabic ? c.DescriptionAr : c.DescriptionEn,
                    ImageUrl = c.ImageUrl,
                    HasSubCategories = c.SubCategories.Any(),
                })
                .ToListAsync();

            var result = new
            {
                ParentCategory = new
                {
                    parentCategory.Id,
                    Name = isArabic ? parentCategory.NameAr : parentCategory.NameEn,
                },
                SubCategories = new PaginatedResult<CategoryListItemDto>
                {
                    Items = subCategories,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                },
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب الفئات الفرعية بنجاح",
                    English = "Subcategories retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب الفئات الفرعية",
                    English = "Failed to retrieve subcategories",
                },
                ex
            );
        }
    }

    private CategoryTreeDto MapToCategoryTreeDto(Category category, bool isArabic)
    {
        return new CategoryTreeDto
        {
            Id = category.Id,
            Name = isArabic ? category.NameAr : category.NameEn,
            // Description = isArabic ? category.DescriptionAr : category.DescriptionEn,
            ImageUrl = category.ImageUrl,
            SubCategories = category
                .SubCategories.Select(sc => MapToCategoryTreeDto(sc, isArabic))
                .ToList(),
        };
    }

    [HttpGet("all-categories")]
    [ProducesResponseType<List<CustomerCategoryDto>>(200)]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";
            var categories = await _context
                .Categories.Include(c => c.SubCategories)
                .Where(c => !c.IsDeleted && c.IsActive)
                .Where(c => c.ParentId == null)
                .Select(c => new CustomerCategoryDto
                {
                    Id = c.Id,
                    Name = isArabic ? c.NameAr : c.NameEn,
                    // Description = isArabic ? c.DescriptionAr : c.DescriptionEn,
                    ImageUrl = c.ImageUrl,
                    NumberOfOffers = c.Offers.Count,
                    SubCategories = c
                        .SubCategories.Select(sc => new CustomerCategoryDto
                        {
                            Id = sc.Id,
                            Name = isArabic ? sc.NameAr : sc.NameEn,
                            // Description = isArabic ? sc.DescriptionAr : sc.DescriptionEn,
                            ImageUrl = sc.ImageUrl,
                            NumberOfOffers = sc.Offers.Count,
                        })
                        .ToList(),
                })
                .OrderByDescending(c => c.NumberOfOffers)
                .ThenBy(c => c.Id)
                .ToListAsync();

            return Represent(
                categories,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب الفئات بنجاح",
                    English = "Categories retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب الفئات",
                    English = "Failed to retrieve categories",
                },
                ex
            );
        }
    }
}

public class CustomerCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int NumberOfOffers { get; set; }
    public List<CustomerCategoryDto> SubCategories { get; set; } = [];
}

public class CategoryTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<CategoryTreeDto> SubCategories { get; set; } = [];
}

public class CategoryListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool HasSubCategories { get; set; }
}

public class GetSubCategoriesDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

public class GetCategoryTreeDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
