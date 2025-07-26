using Mazad.Api.Controllers;
using Mazad.Core.Domain.Users.Authentication;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly MazadDbContext _context;
    private readonly IOtpService _otpService;
    private readonly JwtService _jwtService;

    public UserController(MazadDbContext context, IOtpService otpService, JwtService jwtService)
    {
        _context = context;
        _otpService = otpService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        try
        {
            // Check if phone number already exists
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "رقم الهاتف مستخدم بالفعل",
                        English = "Phone number already registered",
                    }
                );
            }

            // Create new user
            var user = new User
            {
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                // ProfilePhotoUrl = request.ProfilePhotoUrl,
                UserType = UserType.User,
            };

            // Add to database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var otp = await _otpService.GenerateOtp(
                new GenerateOtpRequest { UserId = user.Id.ToString() }
            );

            return Represent(
                user,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تسجيل المستخدم بنجاح",
                    English = "User registered successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل تسجيل المستخدم",
                    English = "Failed to register user",
                },
                ex
            );
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        try
        {
            // Find user by phone number
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PhoneNumber == request.PhoneNumber
            );

            if (user == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "رقم الهاتف غير مسجل",
                        English = "Phone number not registered",
                    }
                );
            }

            // Generate OTP for login verification
            var otp = await _otpService.GenerateOtp(
                new GenerateOtpRequest { UserId = user.Id.ToString() }
            );

            return Represent(
                new { userId = user.Id },
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إرسال رمز التحقق",
                    English = "Verification code has been sent",
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

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
    {
        try
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المستخدم غير موجود",
                        English = "User not found",
                    }
                );
            }

            var verificationResult = await _otpService.ValidateOtp(
                new ValidateOtpRequest { UserId = user.Id.ToString(), Otp = request.Otp }
            );

            if (!verificationResult.IsValid)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "رمز التحقق غير صحيح",
                        English = "Invalid verification code",
                    }
                );
            }
            var token = _jwtService.GenerateToken(user.Id, user.Name, user.UserType);

            return Represent(
                new { token, user },
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
                new LocalizedMessage
                {
                    Arabic = "فشل التحقق من الرمز",
                    English = "Failed to verify code",
                },
                ex
            );
        }
    }

    [HttpGet("profile")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            bool isArabic = GetLanguage() == "ar";
            var userId = GetUserId();

            // Get user basic info
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المستخدم غير موجود",
                        English = "User not found",
                    }
                );
            }

            // Get users that current user follows
            var followedUsers = await _context
                .Followers.Where(f => f.FollowerId == userId)
                .Include(f => f.TheFollowed)
                .Select(f => new UserListDto
                {
                    Id = f.TheFollowed.Id,
                    Name = f.TheFollowed.Name,
                    PhoneNumber = f.TheFollowed.PhoneNumber,
                    ProfilePhotoUrl = f.TheFollowed.ProfilePhotoUrl,
                })
                .ToListAsync();

            // Get user offers
            var userOffers = await _context
                .Offers.Where(o => o.ProviderId == userId)
                .Include(o => o.Category)
                .Select(o => new OfferDto
                {
                    Id = o.Id,
                    CategoryId = o.CategoryId,
                    CategoryName = isArabic ? o.Category.NameAr : o.Category.NameEn,
                    RegionId = o.RegionId,
                    RegionName = isArabic ? o.Region.NameArabic : o.Region.NameEnglish,
                    MainImageUrl = o.MainImageUrl,
                    CreatedAt = o.CreatedAt,
                    Price = o.Price,
                    Description = o.Description,
                    Name = o.Name,
                })
                .ToListAsync();

            var profileData = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                FollowedUsers = followedUsers,
                Offers = userOffers,
            };

            return Represent(
                profileData,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب معلومات المستخدم بنجاح",
                    English = "User profile retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب معلومات المستخدم",
                    English = "Failed to retrieve user profile",
                },
                ex
            );
        }
    }

    [HttpGet("search")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersDto request)
    {
        try
        {
            // Start with base query for users only (UserType = 2)
            var query = _context.Users.Where(u => u.UserType == UserType.User).AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(searchTerm) || u.PhoneNumber.Contains(searchTerm)
                );
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and get users
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePhotoUrl = u.ProfilePhotoUrl,
                })
                .ToListAsync();

            var result = new PaginatedResult<UserListDto>
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
}

public class RegisterUserDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    // public string ProfilePhotoUrl { get; set; } = string.Empty;
}

public class LoginUserDto
{
    public string PhoneNumber { get; set; } = string.Empty;
}

public class VerifyOtpDto
{
    public int UserId { get; set; }
    public string Otp { get; set; } = string.Empty;
}

public class SearchUsersDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

public class UserListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public List<UserListDto> FollowedUsers { get; set; } = new();
    public List<OfferDto> Offers { get; set; } = new();
}

public class OfferDto
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
}
