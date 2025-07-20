using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Mazad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers.Admin;

[ApiController]
[Route("api/admin/sliders")]
public class AdminSliderController : BaseController
{
    private readonly IWebHostEnvironment _environment;
    private readonly MazadDbContext _context;

    public AdminSliderController(IWebHostEnvironment environment, MazadDbContext context)
    {
        _environment = environment;
        _context = context;
    }

    [HttpPost("create")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateSlider([FromForm] CreateSliderDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";
            if (request.Image == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الرجاء ادخال صورة للسلايدر",
                        English = "Please enter an image for the slider",
                    }
                );
            }
            // save image to folder
            var imageName = Guid.NewGuid().ToString();
            var fileStorageService = new FileStorageService(_environment);
            var imageUrl = await fileStorageService.SaveFileAsync(request.Image, "sliders");

            // Create new slider
            var slider = new Slider
            {
                NameEn = request.NameEn,
                NameAr = request.NameAr,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ImageUrl = imageUrl,
                IsActive = true, // Set default active state
            };

            // Add to database
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = slider.Id,
                Name = isArabic ? slider.NameAr : slider.NameEn,
                ImageUrl = slider.ImageUrl,
                IsActive = slider.IsActive,
                StartDate = slider.StartDate,
                EndDate = slider.EndDate,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء السلايدر بنجاح",
                    English = "Slider created successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إنشاء السلايدر",
                    English = "Failed to create slider",
                },
                ex
            );
        }
    }

    [HttpGet]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetSliders([FromQuery] GetSlidersDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            // Start with base query
            var query = _context.Sliders.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(s =>
                    (isArabic ? s.NameAr : s.NameEn).ToLower().Contains(searchTerm)
                );
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            if (request.IsDeleted.HasValue)
            {
                query = query.Where(s => s.IsDeleted == request.IsDeleted.Value);
            }
            else
            {
                query = query.Where(s => !s.IsDeleted);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var sliders = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SliderListDto
                {
                    Id = s.Id,
                    Name = isArabic ? s.NameAr : s.NameEn,
                    ImageUrl = s.ImageUrl,
                    IsActive = s.IsActive,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    CreatedAt = s.CreatedAt,
                })
                .ToListAsync();

            var result = new PaginatedResult<SliderListDto>
            {
                Items = sliders,
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
                    Arabic = "تم جلب السلايدرات بنجاح",
                    English = "Sliders retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب السلايدرات",
                    English = "Failed to retrieve sliders",
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

            var slider = await _context.Sliders.FindAsync(id);

            if (slider == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "السلايدر غير موجود",
                        English = "Slider not found",
                    }
                );
            }

            // Toggle the IsActive status
            slider.IsActive = !slider.IsActive;

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = slider.Id,
                Name = isArabic ? slider.NameAr : slider.NameEn,
                IsActive = slider.IsActive,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = slider.IsActive
                        ? "تم تفعيل السلايدر بنجاح"
                        : "تم إلغاء تفعيل السلايدر بنجاح",
                    English = slider.IsActive
                        ? "Slider activated successfully"
                        : "Slider deactivated successfully",
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
    public async Task<IActionResult> UpdateSlider(int id, [FromForm] UpdateSliderDto request)
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";

            var slider = await _context.Sliders.FindAsync(id);

            if (slider == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "السلايدر غير موجود",
                        English = "Slider not found",
                    }
                );
            }

            // Update slider properties
            slider.NameEn = request.NameEn;
            slider.NameAr = request.NameAr;
            slider.StartDate = request.StartDate;
            slider.EndDate = request.EndDate;

            if (request.ImageUrl != null)
            {
                var fileStorageService = new FileStorageService(_environment);
                slider.ImageUrl = await fileStorageService.SaveFileAsync(
                    request.ImageUrl,
                    "sliders"
                );
            }

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            var response = new
            {
                Id = slider.Id,
                Name = isArabic ? slider.NameAr : slider.NameEn,
                ImageUrl = slider.ImageUrl,
                IsActive = slider.IsActive,
                StartDate = slider.StartDate,
                EndDate = slider.EndDate,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تحديث السلايدر بنجاح",
                    English = "Slider updated successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديث السلايدر",
                    English = "Failed to update slider",
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
            var slider = await _context.Sliders.FindAsync(id);

            if (slider == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "السلايدر غير موجود",
                        English = "Slider not found",
                    }
                );
            }

            // Soft delete the slider
            slider.IsDeleted = true;

            // Save changes
            await _context.SaveChangesAsync();

            // Return localized response
            return Represent(
                true,
                new LocalizedMessage
                {
                    Arabic = "تم حذف السلايدر بنجاح",
                    English = "Slider deleted successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في حذف السلايدر",
                    English = "Failed to delete slider",
                },
                ex
            );
        }
    }
}

public class GetSlidersDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}
public class SliderListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSliderDto
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IFormFile Image { get; set; }
}

public class UpdateSliderDto
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IFormFile? ImageUrl { get; set; }
}

