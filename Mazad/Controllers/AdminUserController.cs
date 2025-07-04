using System.Security.Cryptography;
using System.Text;
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
public class AdminUserController : BaseController
{
    private readonly MazadDbContext _context;
    private readonly JwtService _jwtService;

    public AdminUserController(MazadDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterAdminDto request)
    {
        try
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "البريد الإلكتروني مستخدم بالفعل",
                        English = "Email already registered",
                    }
                );
            }

            // Hash the password
            var hashedPassword = HashPassword(request.Password);

            // Create new admin user
            var adminUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
            };

            // Add to database
            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            return Represent(
                new
                {
                    adminUser.Id,
                    adminUser.Name,
                    adminUser.Email,
                },
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تسجيل المشرف بنجاح",
                    English = "Admin registered successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل تسجيل المشرف",
                    English = "Failed to register admin",
                },
                ex
            );
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginAdminDto request)
    {
        try
        {
            // Find admin by email
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (admin == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "البريد الإلكتروني غير مسجل",
                        English = "Email not registered",
                    }
                );
            }

            // Verify password
            var hashedPassword = HashPassword(request.Password);
            if (admin.Password != hashedPassword)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "كلمة المرور غير صحيحة",
                        English = "Invalid password",
                    }
                );
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(admin.Id, admin.Name, UserType.Admin);

            return Represent(
                new
                {
                    token,
                    admin = new
                    {
                        admin.Id,
                        admin.Name,
                        admin.Email,
                    },
                },
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تسجيل الدخول بنجاح",
                    English = "Login successful",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage { Arabic = "فشل تسجيل الدخول", English = "Failed to login" },
                ex
            );
        }
    }

    [HttpGet("profile")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var admin = await _context
                .Users.Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Email,
                    a.CreatedAt,
                })
                .FirstOrDefaultAsync(a => a.Id == GetUserId());

            if (admin == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المشرف غير موجود",
                        English = "Admin not found",
                    }
                );
            }

            return Represent(
                admin,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب معلومات المشرف بنجاح",
                    English = "Admin profile retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب معلومات المشرف",
                    English = "Failed to retrieve admin profile",
                },
                ex
            );
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

public class RegisterAdminDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginAdminDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AdminUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
