using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/Notification")]
public class NotificationController : BaseController
{
    private readonly MazadDbContext _context;

    public NotificationController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet("my-notifications")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] NotificationPaginationDto request
    )
    {
        try
        {
            var currentUserId = GetUserId();
            var query = _context
                .Notifications.Where(n => n.UserId == currentUserId)
                .OrderByDescending(n => n.CreatedAt)
                .AsQueryable();

            var notifications = await query
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = GetLanguage() == "ar" ? n.TitleAr : n.TitleEn,
                    Body = GetLanguage() == "ar" ? n.BodyAr : n.BodyEn,
                    ImageUrl = n.ImageUrl,
                    ActionUrl = n.ActionUrl,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    NotificationType = n.NotificationType,
                })
                .ToListAsync();

            var totalCount = await query.CountAsync();
            var result = new NotificationPaginationResultDto
            {
                Notifications = notifications,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب الإشعارات بنجاح",
                    English = "Notifications retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب الإشعارات",
                    English = "Failed to retrieve notifications",
                },
                ex
            );
        }
    }

    [HttpPost("{notificationId}/mark-as-read")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> MarkAsRead(long notificationId)
    {
        try
        {
            var currentUserId = GetUserId();
            var notification = await _context.Notifications.FirstOrDefaultAsync(n =>
                n.Id == notificationId && n.UserId == currentUserId
            );

            if (notification == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الإشعار غير موجود أو ليس لديك صلاحية للوصول إليه",
                        English = "Notification not found or you don't have access to it",
                    }
                );
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Represent(
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تحديد الإشعار كمقروء بنجاح",
                    English = "Notification marked as read successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديد الإشعار كمقروء",
                    English = "Failed to mark notification as read",
                },
                ex
            );
        }
    }

    [HttpPost("mark-all-as-read")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var currentUserId = GetUserId();
            var unreadNotifications = await _context
                .Notifications.Where(n => n.UserId == currentUserId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return Represent(
                true,
                new LocalizedMessage
                {
                    Arabic = "تم تحديد جميع الإشعارات كمقروءة بنجاح",
                    English = "All notifications marked as read successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديد جميع الإشعارات كمقروءة",
                    English = "Failed to mark all notifications as read",
                },
                ex
            );
        }
    }

    [HttpGet("unread-count")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var currentUserId = GetUserId();
            var count = await _context.Notifications.CountAsync(n =>
                n.UserId == currentUserId && !n.IsRead
            );

            return Represent(
                new { UnreadCount = count },
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب عدد الإشعارات غير المقروءة بنجاح",
                    English = "Unread notifications count retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب عدد الإشعارات غير المقروءة",
                    English = "Failed to retrieve unread notifications count",
                },
                ex
            );
        }
    }
}

public class NotificationDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string NotificationType { get; set; }
}

public class NotificationPaginationDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class NotificationPaginationResultDto
{
    public List<NotificationDto> Notifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
