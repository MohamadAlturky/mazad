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
                UserType = UserType.Admin,
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
                    adminUser.UserType,
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

    [HttpGet("users")]
    // [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersDto request)
    {
        try
        {
            // Start with base query
            var query = _context.Users.AsQueryable();

            // Apply filters if provided
            query = query.Where(u => u.UserType == UserType.User);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim();
                query = query.Where(u =>
                    u.Name.Contains(searchTerm)
                    || (u.Email != null && u.Email.Contains(searchTerm))
                    || u.PhoneNumber.Contains(searchTerm)
                );
            }

            // For debugging: Log the SQL query
            var debugQuery = query.ToQueryString();
            Console.WriteLine($"Debug SQL Query: {debugQuery}");

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and get users
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserDetailsDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    ProfilePhotoUrl = u.ProfilePhotoUrl,
                    CreatedAt = u.CreatedAt,
                    IsActive = u.IsActive,
                })
                .ToListAsync();

            // For debugging: Log the results
            Console.WriteLine(
                $"Debug Results: Found {users.Count} users with UserType {request.UserType}"
            );

            var result = new PaginatedResult<UserDetailsDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب المستخدمين بنجاح",
                    English = "Users retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب المستخدمين",
                    English = "Failed to retrieve users",
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

public class GetUsersDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public UserType? UserType { get; set; }
    public string? Search { get; set; }
}

public class UserDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public UserType UserType { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public required bool IsActive { get; set; }
}
