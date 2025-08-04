using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mazad;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class StatisticsController : BaseController
{
    private readonly MazadDbContext _context;

    public StatisticsController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
        var totalUsers = await _context
            .Users.Where(e => e.UserType == Mazad.Models.UserType.User)
            .CountAsync();
        var totalOffers = await _context.Offers.CountAsync();
        var totalCategories = await _context.Categories.CountAsync();
        var totalRegions = await _context.Regions.CountAsync();
        var recentUsers = await _context
            .Users.Where(e => e.UserType == Mazad.Models.UserType.User)
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.CreatedAt,
            })
            .ToListAsync();

        var recentOffers = await _context
            .Offers.OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new
            {
                o.Id,
                o.Name,
                o.Price,
                o.CreatedAt,
            })
            .ToListAsync();

        var mostViewedOffers = await _context
            .Offers.OrderByDescending(o => o.NumberOfViews)
            .Take(5)
            .Select(o => new
            {
                o.Id,
                o.Name,
                o.NumberOfViews,
                o.CreatedAt,
            })
            .ToListAsync();

        var statistics = new
        {
            TotalUsers = totalUsers,
            TotalOffers = totalOffers,
            TotalCategories = totalCategories,
            RecentUsers = recentUsers,
            RecentOffers = recentOffers,
            TotalRegions = totalRegions,
            MostViewedOffers = mostViewedOffers,
        };

        return Represent(
            statistics,
            true,
            new LocalizedMessage
            {
                Arabic = "تم جلب الإحصائيات بنجاح",
                English = "Statistics retrieved successfully",
            }
        );
    }
}
