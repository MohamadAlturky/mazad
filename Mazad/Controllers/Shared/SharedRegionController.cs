using System;
using System.Threading.Tasks;
using Mazad.Api.Controllers;
using Mazad.Core.Domain.Regions;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharedRegionController : BaseController
{
    private readonly MazadDbContext _context;

    public SharedRegionController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetRegionTree([FromQuery] GetRegionTreeDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Build base query for regions
            var query = _context.Regions
                .Include(r => r.SubRegions.Where(sr => !sr.IsDeleted))
                    .ThenInclude(sr => sr.SubRegions.Where(ssr => !ssr.IsDeleted))
                .Where(r => !r.IsDeleted);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                
                // Get IDs of all regions (including subregions) that match the search term
                var matchingRegionIds = await _context.Regions
                    .Where(r => !r.IsDeleted &&
                        (isArabic ? r.NameArabic : r.NameEnglish).ToLower().Contains(searchTerm))
                    .Select(r => r.Id)
                    .ToListAsync();

                // Get IDs of parent regions that have matching subregions
                var parentRegionIds = await _context.Regions
                    .Where(r => !r.IsDeleted && 
                        r.SubRegions.Any(sr => 
                            !sr.IsDeleted && matchingRegionIds.Contains(sr.Id)))
                    .Select(r => r.Id)
                    .ToListAsync();

                // Combine all matching IDs
                var allRelevantIds = matchingRegionIds.Union(parentRegionIds).ToList();

                // Filter root regions that either match the search term or have matching subregions
                query = query.Where(r => r.ParentId == null && allRelevantIds.Contains(r.Id));
            }
            else
            {
                // If no search term, only get root regions
                query = query.Where(r => r.ParentId == null);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Get paginated root regions with their subregions
            var rootRegions = await query
                .OrderBy(r => isArabic ? r.NameArabic : r.NameEnglish)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Map regions to tree structure
            var regionTree = rootRegions.Select(r => MapToRegionTreeDto(r, isArabic)).ToList();

            var result = new PaginatedResult<RegionTreeDto>
            {
                Items = regionTree,
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
                    Arabic = "تم جلب شجرة المناطق بنجاح",
                    English = "Region tree retrieved successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب شجرة المناطق",
                    English = "Failed to retrieve region tree"
                },
                ex
            );
        }
    }

    [HttpGet("{regionId}/subregions")]
    public async Task<IActionResult> GetSubRegions(int regionId, [FromQuery] GetSubRegionsDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Check if the parent region exists
            var parentRegion = await _context.Regions
                .FirstOrDefaultAsync(r => r.Id == regionId && !r.IsDeleted);

            if (parentRegion == null)
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

            // Build query for subregions with their child count
            var query = _context.Regions
                .Include(r => r.SubRegions.Where(sr => !sr.IsDeleted))
                .Where(r => r.ParentId == regionId && !r.IsDeleted);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(r =>
                    (isArabic ? r.NameArabic : r.NameEnglish).ToLower().Contains(searchTerm)
                );
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Get paginated subregions
            var subRegions = await query
                .OrderBy(r => isArabic ? r.NameArabic : r.NameEnglish)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RegionListItemDto
                {
                    Id = r.Id,
                    Name = isArabic ? r.NameArabic : r.NameEnglish,
                    HasSubRegions = r.SubRegions.Any()
                })
                .ToListAsync();

            var result = new
            {
                ParentRegion = new
                {
                    parentRegion.Id,
                    Name = isArabic ? parentRegion.NameArabic : parentRegion.NameEnglish
                },
                SubRegions = new PaginatedResult<RegionListItemDto>
                {
                    Items = subRegions,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                }
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب المناطق الفرعية بنجاح",
                    English = "Subregions retrieved successfully"
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب المناطق الفرعية",
                    English = "Failed to retrieve subregions"
                },
                ex
            );
        }
    }

    [HttpGet("AllRegions")]
    public async Task<IActionResult> GetAllRegions()
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var regions = await _context.Regions
                .Include(r => r.ParentRegion)
                .Where(r => !r.IsDeleted)
                .Select(r => new RegionDto
                {
                    Id = r.Id,
                    Name = isArabic ? r.NameArabic : r.NameEnglish,
                    ParentId = r.ParentId,
                    ParentName = r.ParentRegion != null ? (isArabic ? r.ParentRegion.NameArabic : r.ParentRegion.NameEnglish) : null,
                    HasSubRegions = r.SubRegions.Any(sr => !sr.IsDeleted)
                })
                .OrderBy(r => r.ParentId) // Root regions first, then grouped by parent
                .ThenBy(r => r.Name)      // Alphabetically within each group
                .ToListAsync();

            return Represent(
                regions,
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

    private RegionTreeDto MapToRegionTreeDto(Region region, bool isArabic)
    {
        return new RegionTreeDto
        {
            Id = region.Id,
            Name = isArabic ? region.NameArabic : region.NameEnglish,
            SubRegions = region.SubRegions
                .Select(sr => MapToRegionTreeDto(sr, isArabic))
                .ToList()
        };
    }
}

public class RegionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool HasSubRegions { get; set; }
}

public class RegionTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RegionTreeDto> SubRegions { get; set; } = [];
}

public class RegionListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool HasSubRegions { get; set; }
}

public class GetSubRegionsDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

public class GetRegionTreeDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
} 