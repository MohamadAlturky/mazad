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
public class SharedOfferController : BaseController
{
    private readonly MazadDbContext _context;

    public SharedOfferController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetOffersByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] bool ascending = false)
    {
        try
        {
            // Validate page and pageSize
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50; // Maximum page size

            // Check if category exists
            var categoryExists = await _context.Set<Category>()
                .AnyAsync(c => !c.IsDeleted && c.Id == categoryId);

            if (!categoryExists)
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

            // Build query
            var query = _context.Set<Offer>()
                .Include(o => o.Category)
                .Include(o => o.Region)
                .Where(o => !o.IsDeleted && o.CategoryId == categoryId);

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "price" => ascending 
                    ? query.OrderBy(o => o.Price)
                    : query.OrderByDescending(o => o.Price),
                "name" => ascending
                    ? query.OrderBy(o => o.Name)
                    : query.OrderByDescending(o => o.Name),
                _ => ascending // Default to createdAt
                    ? query.OrderBy(o => o.CreatedAt)
                    : query.OrderByDescending(o => o.CreatedAt)
            };

            // Get total count for pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Get paginated results
            var offers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OfferListItemDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    Price = o.Price,
                    CategoryId = o.CategoryId,
                    CategoryName = o.Category.NameAr,
                    RegionId = o.RegionId,
                    RegionName = o.Region.NameArabic,
                    MainImageUrl = o.MainImageUrl,
                    CreatedAt = o.CreatedAt,
                    IsActive = o.IsActive
                })
                .ToListAsync();

            var result = new PaginatedResult<OfferListItemDto>
            {
                Items = offers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب العروض بنجاح",
                    English = "Offers retrieved successfully"
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving offers: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب العروض",
                    English = "Failed to retrieve offers"
                },
                ex
            );
        }
    }
}

public class OfferListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}