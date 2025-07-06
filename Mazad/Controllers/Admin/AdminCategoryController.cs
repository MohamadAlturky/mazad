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
[Route("api/[controller]")]
public class AdminCategoryController : BaseController
{
    private readonly MazadDbContext _context;

    public AdminCategoryController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Start with base query
            var query = _context.Categories
                .Include(c => c.ParentCategory)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(c =>
                    (isArabic ? c.NameAr : c.NameEn).ToLower().Contains(searchTerm) ||
                    (isArabic ? c.DescriptionAr : c.DescriptionEn).ToLower().Contains(searchTerm)
                );
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            if (request.IsDeleted.HasValue)
            {
                query = query.Where(c => c.IsDeleted == request.IsDeleted.Value);
            }

            if (request.ParentId.HasValue)
            {
                query = query.Where(c => c.ParentId == request.ParentId.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var categories = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = isArabic ? c.NameAr : c.NameEn,
                    Description = isArabic ? c.DescriptionAr : c.DescriptionEn,
                    ImageUrl = c.ImageUrl,
                    IsActive = c.IsActive,
                    IsDeleted = c.IsDeleted,
                    ParentId = c.ParentId,
                    ParentName = c.ParentCategory != null ? (isArabic ? c.ParentCategory.NameAr : c.ParentCategory.NameEn) : null,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            var result = new PaginatedResult<CategoryListDto>
            {
                Items = categories,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب الفئات بنجاح",
                    English = "Categories retrieved successfully"
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
                    English = "Failed to retrieve categories"
                },
                ex
            );
        }
    }

    [HttpPost("create")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Validate parent category if specified
            if (request.ParentId.HasValue)
            {
                var parentExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.ParentId.Value && !c.IsDeleted);

                if (!parentExists)
                {
                    return Represent(
                        false,
                        new LocalizedMessage
                        {
                            Arabic = "الفئة الأم غير موجودة",
                            English = "Parent category not found"
                        }
                    );
                }
            }

            // Create new category
            var category = new Category
            {
                NameEn = request.NameEn,
                NameAr = request.NameAr,
                DescriptionEn = request.DescriptionEn,
                DescriptionAr = request.DescriptionAr,
                ImageUrl = request.ImageUrl,
                ParentId = request.ParentId,
                IsActive = true // Set default active state
            };

            // Add to database
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = category.Id,
                Name = isArabic ? category.NameAr : category.NameEn,
                Description = isArabic ? category.DescriptionAr : category.DescriptionEn,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                ParentId = category.ParentId
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء الفئة بنجاح",
                    English = "Category created successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إنشاء الفئة",
                    English = "Failed to create category"
                },
                ex
            );
        }
    }

    [HttpPut("update/{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الفئة غير موجودة",
                        English = "Category not found"
                    }
                );
            }

            // Update category properties
            category.NameEn = request.NameEn;
            category.NameAr = request.NameAr;
            category.DescriptionEn = request.DescriptionEn;
            category.DescriptionAr = request.DescriptionAr;
            category.ImageUrl = request.ImageUrl;
            category.ParentId = request.ParentId;

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = category.Id,
                Name = isArabic ? category.NameAr : category.NameEn,
                Description = isArabic ? category.DescriptionAr : category.DescriptionEn,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                ParentId = category.ParentId
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تحديث الفئة بنجاح",
                    English = "Category updated successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديث الفئة",
                    English = "Failed to update category"
                },
                ex
            );
        }
    }

    [HttpPatch("toggle-activation/{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ToggleActivation(int id)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الفئة غير موجودة",
                        English = "Category not found"
                    }
                );
            }

            // Toggle the IsActive status
            category.IsActive = !category.IsActive;

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = category.Id,
                Name = isArabic ? category.NameAr : category.NameEn,
                IsActive = category.IsActive
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = category.IsActive ? "تم تفعيل الفئة بنجاح" : "تم إلغاء تفعيل الفئة بنجاح",
                    English = category.IsActive ? "Category activated successfully" : "Category deactivated successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تغيير حالة التفعيل",
                    English = "Failed to toggle activation status"
                },
                ex
            );
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الفئة غير موجودة",
                        English = "Category not found"
                    }
                );
            }

            // Soft delete the category and all its subcategories
            category.IsDeleted = true;
            foreach (var subCategory in category.SubCategories)
            {
                subCategory.IsDeleted = true;
            }

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = category.Id,
                Name = isArabic ? category.NameAr : category.NameEn,
                IsDeleted = category.IsDeleted,
                DeletedSubcategoriesCount = category.SubCategories.Count
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم حذف الفئة بنجاح",
                    English = "Category deleted successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في حذف الفئة",
                    English = "Failed to delete category"
                },
                ex
            );
        }
    }
}

public class GetCategoriesDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public int? ParentId { get; set; }
}

public class CategoryListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CreateCategoryDto
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}

public class UpdateCategoryDto
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}
