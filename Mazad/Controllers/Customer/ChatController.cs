using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : BaseController
{
    private readonly MazadDbContext _context;

    public ChatController(MazadDbContext context)
    {
        _context = context;
    }

    [HttpGet("with-user/{userId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetChatWithUser(int userId)
    {
        try
        {
            var currentUserId = GetUserId();

            // Check if user exists
            var otherUser = await _context.Users.FindAsync(userId);
            if (otherUser == null)
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

            // Find existing chat between these users
            var chat = await _context
                .Chats.Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == currentUserId && c.User2Id == userId)
                    || (c.User1Id == userId && c.User2Id == currentUserId)
                );

            if (chat == null)
            {
                // Create new chat
                chat = new Chat
                {
                    User1Id = currentUserId,
                    User2Id = userId,
                    Messages = new List<ChatMessage>(),
                };

                await _context.Chats.AddAsync(chat);
                await _context.SaveChangesAsync();
            }

            var chatDto = new ChatDto
            {
                Id = chat.Id,
                User = new UserListDto
                {
                    Id = chat.User1.Id == currentUserId ? chat.User2.Id : chat.User1.Id,
                    Name = chat.User1.Id == currentUserId ? chat.User2.Name : chat.User1.Name,
                    PhoneNumber =
                        chat.User1.Id == currentUserId
                            ? chat.User2.PhoneNumber
                            : chat.User1.PhoneNumber,
                    ProfilePhotoUrl =
                        chat.User1.Id == currentUserId
                            ? chat.User2.ProfilePhotoUrl
                            : chat.User1.ProfilePhotoUrl,
                },
            };

            return Represent(
                chatDto,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب المحادثة بنجاح",
                    English = "Chat retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب المحادثة",
                    English = "Failed to retrieve chat",
                },
                ex
            );
        }
    }

    [HttpGet("{chatId}/messages")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetChatMessages(
        int chatId,
        [FromQuery] MessageCursorPaginationDto request
    )
    {
        try
        {
            var currentUserId = GetUserId();

            // Verify chat exists and user is part of it
            var chat = await _context.Chats.FirstOrDefaultAsync(c =>
                c.Id == chatId && (c.User1Id == currentUserId || c.User2Id == currentUserId)
            );

            if (chat == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المحادثة غير موجودة أو ليس لديك صلاحية للوصول إليها",
                        English = "Chat not found or you don't have access to it",
                    }
                );
            }

            // Build query for messages
            var query = _context
                .ChatMessages.Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.Date)
                .AsQueryable();

            // Apply cursor pagination
            if (request.LastMessageId.HasValue)
            {
                var lastMessage = await _context.ChatMessages.FindAsync(
                    request.LastMessageId.Value
                );
                if (lastMessage != null)
                {
                    query = query.Where(m => m.Date < lastMessage.Date);
                }
            }

            // Get messages with pagination
            var messages = await query
                .Take(request.PageSize)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    Date = m.Date,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    IsRead = m.IsRead,
                    ReadDate = m.ReadDate,
                })
                .ToListAsync();

            // Mark unread messages as read if current user is the receiver
            var unreadMessages = await _context
                .ChatMessages.Where(m =>
                    m.ChatId == chatId && m.ReceiverId == currentUserId && !m.IsRead
                )
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                    message.ReadDate = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }

            // Determine if there are more messages
            bool hasMoreMessages = await query.Skip(request.PageSize).AnyAsync();

            var result = new MessageCursorPaginationResultDto
            {
                Messages = messages,
                HasMore = hasMoreMessages,
                LastMessageId = messages.LastOrDefault()?.Id,
            };

            return Represent(
                result,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب الرسائل بنجاح",
                    English = "Messages retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب الرسائل",
                    English = "Failed to retrieve messages",
                },
                ex
            );
        }
    }

    [HttpPost("{chatId}/send")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> SendMessage(int chatId, [FromBody] SendMessageDto request)
    {
        try
        {
            var currentUserId = GetUserId();

            // Verify chat exists and user is part of it
            var chat = await _context
                .Chats.Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c =>
                    c.Id == chatId && (c.User1Id == currentUserId || c.User2Id == currentUserId)
                );

            if (chat == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المحادثة غير موجودة أو ليس لديك صلاحية للوصول إليها",
                        English = "Chat not found or you don't have access to it",
                    }
                );
            }

            // Determine receiver ID
            int receiverId = chat.User1Id == currentUserId ? chat.User2Id : chat.User1Id;

            // Create new message
            var message = new ChatMessage
            {
                ChatId = chatId,
                Message = request.Message,
                Date = DateTime.UtcNow,
                SenderId = currentUserId,
                ReceiverId = receiverId,
                IsRead = false,
            };

            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            var messageDto = new ChatMessageDto
            {
                Id = message.Id,
                Message = message.Message,
                Date = message.Date,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                IsRead = message.IsRead,
                ReadDate = message.ReadDate,
            };

            return Represent(
                messageDto,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إرسال الرسالة بنجاح",
                    English = "Message sent successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إرسال الرسالة",
                    English = "Failed to send message",
                },
                ex
            );
        }
    }
}

public class ChatDto
{
    public int Id { get; set; }
    public UserListDto User { get; set; }
}

public class ChatMessageDto
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
}

public class MessageCursorPaginationDto
{
    public int? LastMessageId { get; set; }
    public int PageSize { get; set; } = 20;
}

public class MessageCursorPaginationResultDto
{
    public List<ChatMessageDto> Messages { get; set; } = new();
    public bool HasMore { get; set; }
    public int? LastMessageId { get; set; }
}

public class SendMessageDto
{
    public string Message { get; set; } = string.Empty;
}
