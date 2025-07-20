using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers.Customer;

[ApiController]
[Route("api/customer/sliders")]
public class CustomerSliderController : BaseController
{
    private readonly MazadDbContext _context;

    public CustomerSliderController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveSliders()
    {
        try
        {
            var currentLanguage = GetLanguage();
            var isArabic = currentLanguage == "ar";
            var now = DateTime.UtcNow;

            var sliders = await _context
                .Sliders.Where(s =>
                    s.IsActive && !s.IsDeleted && s.StartDate <= now && s.EndDate >= now
                )
                .OrderByDescending(s => s.Id)
                .Select(s => new SliderListDto
                {
                    Name = isArabic ? s.NameAr : s.NameEn,
                    ImageUrl = s.ImageUrl,
                })
                .ToListAsync();

            return Represent(
                sliders,
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
}

public class SliderListDto
{
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
