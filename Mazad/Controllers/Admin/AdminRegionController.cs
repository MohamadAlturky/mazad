using System;
using System.Threading.Tasks;
using Mazad.Api.Controllers;
using Mazad.Core.Domain.Regions;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminRegionController : BaseController
{
    private readonly MazadDbContext _context;

    public AdminRegionController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetRegions([FromQuery] GetRegionsDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Start with base query
            var query = _context.Regions
                .Include(r => r.ParentRegion)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(r =>
                    (isArabic ? r.NameArabic : r.NameEnglish).ToLower().Contains(searchTerm)
                );
            }

            if (request.IsDeleted.HasValue)
            {
                query = query.Where(r => r.IsDeleted == request.IsDeleted.Value);
            }

            if (request.ParentId.HasValue)
            {
                query = query.Where(r => r.ParentId == request.ParentId.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var regions = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RegionListDto
                {
                    Id = r.Id,
                    Name = isArabic ? r.NameArabic : r.NameEnglish,
                    ParentId = r.ParentId,
                    ParentName = r.ParentRegion != null ? (isArabic ? r.ParentRegion.NameArabic : r.ParentRegion.NameEnglish) : null,
                    CreatedAt = r.CreatedAt,
                    IsDeleted = r.IsDeleted
                })
                .ToListAsync();

            var result = new PaginatedResult<RegionListDto>
            {
                Items = regions,
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
                    Arabic = "تم جلب المناطق بنجاح",
                    English = "Regions retrieved successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب المناطق",
                    English = "Failed to retrieve regions"
                },
                ex
            );
        }
    }

    [HttpPost("create")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateRegion([FromBody] CreateRegionDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Validate parent region if specified
            if (request.ParentId.HasValue)
            {
                var parentExists = await _context.Regions
                    .AnyAsync(r => r.Id == request.ParentId.Value && !r.IsDeleted);

                if (!parentExists)
                {
                    return Represent(
                        false,
                        new LocalizedMessage
                        {
                            Arabic = "المنطقة الأم غير موجودة",
                            English = "Parent region not found"
                        }
                    );
                }
            }

            // Create new region
            var region = new Region
            {
                NameEnglish = request.NameEnglish,
                NameArabic = request.NameArabic,
                ParentId = request.ParentId
            };

            // Add to database
            await _context.Regions.AddAsync(region);
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = region.Id,
                Name = isArabic ? region.NameArabic : region.NameEnglish,
                ParentId = region.ParentId
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء المنطقة بنجاح",
                    English = "Region created successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إنشاء المنطقة",
                    English = "Failed to create region"
                },
                ex
            );
        }
    }

    [HttpPut("update/{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateRegionDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var region = await _context.Regions.FindAsync(id);

            if (region == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المنطقة غير موجودة",
                        English = "Region not found"
                    }
                );
            }

            // Update region properties
            region.NameEnglish = request.NameEnglish;
            region.NameArabic = request.NameArabic;
            region.ParentId = request.ParentId;

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = region.Id,
                Name = isArabic ? region.NameArabic : region.NameEnglish,
                ParentId = region.ParentId
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تحديث المنطقة بنجاح",
                    English = "Region updated successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديث المنطقة",
                    English = "Failed to update region"
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

            var region = await _context.Regions
                .Include(r => r.SubRegions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (region == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المنطقة غير موجودة",
                        English = "Region not found"
                    }
                );
            }

            // Soft delete the region and all its subregions
            region.IsDeleted = true;
            foreach (var subRegion in region.SubRegions)
            {
                subRegion.IsDeleted = true;
            }

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = region.Id,
                Name = isArabic ? region.NameArabic : region.NameEnglish,
                IsDeleted = region.IsDeleted,
                DeletedSubregionsCount = region.SubRegions.Count
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم حذف المنطقة بنجاح",
                    English = "Region deleted successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في حذف المنطقة",
                    English = "Failed to delete region"
                },
                ex
            );
        }
    }
}

public class GetRegionsDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsDeleted { get; set; }
    public int? ParentId { get; set; }
}

public class RegionListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRegionDto
{
    public string NameEnglish { get; set; } = string.Empty;
    public string NameArabic { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}

public class UpdateRegionDto
{
    public string NameEnglish { get; set; } = string.Empty;
    public string NameArabic { get; set; } = string.Empty;
    public int? ParentId { get; set; }
} 