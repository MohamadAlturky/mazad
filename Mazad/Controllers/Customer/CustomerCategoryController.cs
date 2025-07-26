using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mazad.Api.Controllers;
using Mazad.Core.Domain.Users.Authentication;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Mazad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/customer/categories")]
public class CustomerCategoryController : BaseController
{
    private readonly IWebHostEnvironment _environment;
    private readonly MazadDbContext _context;

    public CustomerCategoryController(MazadDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet("dropdown")]
    public async Task<IActionResult> GetDropdown()
    {
        var currentLanguage = GetLanguage();
        var isArabic = currentLanguage == "ar";
        var categories = await _context
            .Categories.Where(c => !c.IsDeleted && c.ParentId != null)
            .Select(c => new { Id = c.Id, Name = isArabic ? c.NameAr : c.NameEn })
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
}
