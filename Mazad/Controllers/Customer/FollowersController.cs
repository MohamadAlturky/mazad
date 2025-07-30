using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers.Customer;

[ApiController]
[Route("api/followers")]
public class FollowersController : BaseController
{
    private readonly MazadDbContext _context;

    public FollowersController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet("following")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetFollowing()
    {
        try
        {
            var currentUserId = GetUserId();

            var followedUsers = await _context
                .Followers.Where(f => f.FollowerId == currentUserId)
                .Include(f => f.TheFollowed)
                .Select(f => new UserListDto
                {
                    Id = f.TheFollowed.Id,
                    Name = f.TheFollowed.Name,
                    PhoneNumber = f.TheFollowed.PhoneNumber,
                    ProfilePhotoUrl = f.TheFollowed.ProfilePhotoUrl,
                })
                .ToListAsync();

            return Represent(
                followedUsers,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب المتابعين بنجاح",
                    English = "Following users retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب المتابعين",
                    English = "Failed to retrieve following users",
                },
                ex
            );
        }
    }

    [HttpPost("toggle/{userId:int}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> ToggleFollow(int userId)
    {
        try
        {
            var currentUserId = GetUserId();

            // Check if user is trying to follow themselves
            if (currentUserId == userId)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "لا يمكنك متابعة نفسك",
                        English = "You cannot follow yourself",
                    }
                );
            }

            // Check if the user to follow exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted);
            if (!userExists)
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

            // Check if follow relationship already exists
            var existingFollowRelation = await _context.Followers.FirstOrDefaultAsync(f =>
                f.FollowerId == currentUserId && f.FollowedId == userId
            );

            bool isNowFollowing;
            string actionArabic;
            string actionEnglish;

            if (existingFollowRelation != null)
            {
                // Unfollow: Remove the relationship
                _context.Followers.Remove(existingFollowRelation);
                isNowFollowing = false;
                actionArabic = "تم إلغاء المتابعة بنجاح";
                actionEnglish = "Successfully unfollowed user";
            }
            else
            {
                // Follow: Create new relationship
                var newFollowRelation = new Follower
                {
                    FollowerId = currentUserId,
                    FollowedId = userId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _context.Followers.AddAsync(newFollowRelation);
                isNowFollowing = true;
                actionArabic = "تم إضافة المتابعة بنجاح";
                actionEnglish = "Successfully followed user";
            }

            await _context.SaveChangesAsync();

            return Represent(
                new { isFollowing = isNowFollowing },
                true,
                new LocalizedMessage { Arabic = actionArabic, English = actionEnglish }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديث حالة المتابعة",
                    English = "Failed to toggle follow status",
                },
                ex
            );
        }
    }
}
