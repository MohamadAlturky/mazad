using Mazad.Api.Controllers;
using Mazad.Core.Domain.Regions;
using Mazad.Core.Shared.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers.Admin;

[ApiController]
[Route("api/admin/regions")]
public class AdminRegionController : BaseController
{
    private readonly MazadDbContext _context;

    public AdminRegionController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateRegion([FromBody] CreateRegionDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var region = new Region
            {
                NameEnglish = request.NameEnglish,
                NameArabic = request.NameArabic,
                ParentId = request.ParentId,
                IsActive = true,
            };

            await _context.Regions.AddAsync(region);
            await _context.SaveChangesAsync();

            var response = new
            {
                Id = region.Id,
                Name = isArabic ? region.NameArabic : region.NameEnglish,
                IsActive = region.IsActive,
                ParentId = region.ParentId,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء المنطقة بنجاح",
                    English = "Region created successfully",
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
                    English = "Failed to create region",
                },
                ex
            );
        }
    }

    [HttpGet]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetRegions([FromQuery] GetRegionsDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var query = _context.Regions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(r =>
                    (isArabic ? r.NameArabic : r.NameEnglish).ToLower().Contains(searchTerm)
                );
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == request.IsActive.Value);
            }

            if (request.IsDeleted.HasValue)
            {
                query = query.Where(r => r.IsDeleted == request.IsDeleted.Value);
            }
            else
            {
                query = query.Where(r => !r.IsDeleted);
            }

            var totalCount = await query.CountAsync();

            var regions = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RegionListDto
                {
                    Id = r.Id,
                    Name = isArabic ? r.NameArabic : r.NameEnglish,
                    IsActive = r.IsActive,
                    ParentId = r.ParentId,
                    ParentName =
                        r.ParentRegion != null
                            ? (isArabic ? r.ParentRegion.NameArabic : r.ParentRegion.NameEnglish)
                            : null,
                })
                .ToListAsync();

            var result = new PaginatedResult<RegionListDto>
            {
                Items = regions,
                TotalCount = totalCount,
                Page = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب المناطق بنجاح",
                    English = "Regions retrieved successfully",
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
                    English = "Failed to retrieve regions",
                },
                ex
            );
        }
    }

    [HttpGet("dropdown")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetRegionsForDropdown()
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var regions = await _context
                .Regions.Where(r => r.IsActive && !r.IsDeleted)
                .OrderByDescending(r => r.Id)
                .Select(r => new RegionDropdownDto
                {
                    Id = r.Id,
                    Name = isArabic ? r.NameArabic : r.NameEnglish,
                })
                .ToListAsync();

            return Represent(
                regions,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب المناطق بنجاح",
                    English = "Regions retrieved successfully",
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
                    English = "Failed to retrieve regions",
                },
                ex
            );
        }
    }

    [HttpGet("tree")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetRegionsTree()
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var allRegions = await _context
                .Regions.Where(r => r.IsActive && !r.IsDeleted)
                .OrderBy(r => r.Id)
                .Select(r => new
                {
                    r.Id,
                    Name = isArabic ? r.NameArabic : r.NameEnglish,
                    r.ParentId,
                })
                .ToListAsync();

            var regionMap = allRegions.ToDictionary(
                r => r.Id,
                r => new RegionTreeNodeDto { Id = r.Id, Name = r.Name }
            );

            var tree = new List<RegionTreeNodeDto>();

            foreach (var region in allRegions)
            {
                if (
                    region.ParentId.HasValue
                    && regionMap.TryGetValue(region.ParentId.Value, out var parentRegion)
                )
                {
                    parentRegion.SubRegions.Add(regionMap[region.Id]);
                }
                else
                {
                    tree.Add(regionMap[region.Id]);
                }
            }

            return Represent(
                tree,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب شجرة المناطق بنجاح",
                    English = "Regions tree retrieved successfully",
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
                    English = "Failed to retrieve regions tree",
                },
                ex
            );
        }
    }

    [HttpPatch("toggle-activation/{id}")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ToggleActivation(int id)
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
                        English = "Region not found",
                    }
                );
            }

            region.IsActive = !region.IsActive;

            await _context.SaveChangesAsync();

            var response = new
            {
                Id = region.Id,
                Name = isArabic ? region.NameArabic : region.NameEnglish,
                IsActive = region.IsActive,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = region.IsActive
                        ? "تم تفعيل المنطقة بنجاح"
                        : "تم إلغاء تفعيل المنطقة بنجاح",
                    English = region.IsActive
                        ? "Region activated successfully"
                        : "Region deactivated successfully",
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
                    English = "Failed to toggle activation status",
                },
                ex
            );
        }
    }

    [HttpPut("update/{id}")]
    // [Authorize(Policy = "Admin")]
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
                        English = "Region not found",
                    }
                );
            }

            region.NameEnglish = request.NameEnglish;
            region.NameArabic = request.NameArabic;
            region.ParentId = request.ParentId;

            await _context.SaveChangesAsync();

            var response = new
            {
                Id = region.Id,
                Name = isArabic ? region.NameArabic : region.NameEnglish,
                IsActive = region.IsActive,
                ParentId = region.ParentId,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تحديث المنطقة بنجاح",
                    English = "Region updated successfully",
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
                    English = "Failed to update region",
                },
                ex
            );
        }
    }

    [HttpDelete("delete/{id}")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var region = await _context.Regions.FindAsync(id);

            if (region == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المنطقة غير موجودة",
                        English = "Region not found",
                    }
                );
            }

            region.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Represent(
                true,
                new LocalizedMessage
                {
                    Arabic = "تم حذف المنطقة بنجاح",
                    English = "Region deleted successfully",
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
                    English = "Failed to delete region",
                },
                ex
            );
        }
    }
}

public class GetRegionsDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class RegionListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public required string? ParentName { get; set; }
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

public class RegionDropdownDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RegionTreeNodeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RegionTreeNodeDto> SubRegions { get; set; } = new List<RegionTreeNodeDto>();
}
