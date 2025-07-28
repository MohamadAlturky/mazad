using Mazad.Api.Controllers;
using Mazad.Core.Domain.Regions;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.Interfaces;
using Mazad.Models;
using Mazad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/offers")]
public class CustomerOfferController : BaseController
{
    private readonly MazadDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public CustomerOfferController(MazadDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOfferById(int id)
    {
        bool isArabic = GetLanguage() == "ar";
        try
        {
            var offer = await _context
                .Set<Offer>()
                .Include(o => o.Category)
                .Include(o => o.Region)
                .Include(o => o.Provider)
                .Include(o => o.ImagesUrl.Where(i => !i.IsDeleted))
                .Where(o => !o.IsDeleted && o.IsActive && o.Id == id)
                .Select(o => new OfferDetailsPageDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    Price = o.Price,
                    CategoryId = o.CategoryId,
                    CategoryName = isArabic ? o.Category.NameAr : o.Category.NameEn,
                    RegionId = o.RegionId,
                    RegionName = isArabic ? o.Region.NameArabic : o.Region.NameEnglish,
                    MainImageUrl = o.MainImageUrl,
                    AdditionalImages = o.ImagesUrl.Select(i => i.ImageUrl).ToList(),
                    CreatedAt = o.CreatedAt,
                    IsActive = o.IsActive,
                    ProviderId = o.ProviderId,
                    ProviderName = o.Provider.Name,
                    ProviderPhoneNumber = o.Provider.PhoneNumber,
                    ProviderProfilePhotoUrl = o.Provider.ProfilePhotoUrl,
                    ProviderCreatedAt = o.Provider.CreatedAt.ToString("yyyy-MM-dd"),
                    IsFavorite = o.Favorites.Any(f => f.UserId == GetUserId()),
                    NumberOfFavorites = o.Favorites.Count,
                    NumberOfViews = o.NumberOfViews,
                })
                .FirstOrDefaultAsync();

            if (offer == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage { Arabic = "العرض غير موجود", English = "Offer not found" }
                );
            }

            await _context
                .Offers.Where(e => e.Id == id)
                .ExecuteUpdateAsync(e =>
                    e.SetProperty(o => o.NumberOfViews, o => o.NumberOfViews + 1)
                );
            await _context.SaveChangesAsync();

            return Represent(
                offer,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم استرجاع تفاصيل العرض بنجاح",
                    English = "Offer details retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving offer details: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في استرجاع تفاصيل العرض",
                    English = "Failed to retrieve offer details",
                },
                ex
            );
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOffers([FromQuery] GetOffersRequestDto request)
    {
        bool isArabic = GetLanguage() == "ar";
        try
        {
            // Default limit if not provided or invalid
            var limit = request.Limit <= 0 ? 10 : Math.Min(request.Limit, 50);

            // Start with the base query
            var query = _context
                .Set<Offer>()
                .Include(o => o.Category)
                .Include(o => o.Region)
                .Where(o => !o.IsDeleted && o.IsActive);

            // Apply category filter if provided
            if (request.CategoryId > 0)
            {
                query = query.Where(o => o.CategoryId == request.CategoryId);
            }

            // Apply region filter if provided
            if (request.RegionId > 0)
            {
                query = query.Where(o => o.RegionId == request.RegionId);
            }

            // Apply price range filter if provided
            if (request.MinPrice > 0)
            {
                query = query.Where(o => o.Price >= request.MinPrice);
            }

            if (request.MaxPrice > 0)
            {
                query = query.Where(o => o.Price <= request.MaxPrice);
            }

            // Apply search term if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(o =>
                    o.Name.ToLower().Contains(searchTerm)
                    || o.Description.ToLower().Contains(searchTerm)
                );
            }

            // Apply cursor pagination
            if (request.Cursor.HasValue)
            {
                query = request.SortDescending
                    ? query.Where(o => o.Id < request.Cursor) // Get items before cursor for descending order
                    : query.Where(o => o.Id > request.Cursor); // Get items after cursor for ascending order
            }

            // Apply ordering
            query = request.SortDescending
                ? query.OrderByDescending(o => o.Id)
                : query.OrderBy(o => o.Id);

            // Execute query with limit
            var offers = await query
                .Take(limit + 1) // Get one extra item to determine if there are more results
                .Select(o => new OfferPaginatedItemDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    Price = o.Price,
                    CategoryId = o.CategoryId,
                    CategoryName = isArabic ? o.Category.NameAr : o.Category.NameEn,
                    RegionId = o.RegionId,
                    RegionName = isArabic ? o.Region.NameArabic : o.Region.NameEnglish,
                    MainImageUrl = o.MainImageUrl,
                    CreatedAt = o.CreatedAt,
                    NumberOfComments = o.Comments.Count,
                    NumberOfFavorites = o.Favorites.Count,
                    NumberOfViews = o.NumberOfViews,
                })
                .ToListAsync();

            // Check if there are more results
            var hasMore = offers.Count > limit;
            if (hasMore)
            {
                offers.RemoveAt(offers.Count - 1); // Remove the extra item
            }

            // Get the next cursor (use int? for nullable int)
            int? nextCursor = offers.Count > 0 ? offers[offers.Count - 1].Id : null;

            var response = new GetOffersResponseDto
            {
                Items = offers,
                HasMore = hasMore,
                NextCursor = nextCursor,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم استرجاع العروض بنجاح",
                    English = "Offers retrieved successfully",
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
                    Arabic = "فشل في استرجاع العروض",
                    English = "Failed to retrieve offers",
                },
                ex
            );
        }
    }

    [HttpPost("create")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> CreateOffer([FromForm] CreateOfferDto request)
    {
        await _context.Database.BeginTransactionAsync();

        try
        {
            // Input validation
            if (request.CategoryId <= 0)
            {
                Console.WriteLine($"Invalid CategoryId provided: {request.CategoryId}");
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "معرف الفئة غير صالح",
                        English = "Invalid category ID",
                    }
                );
            }

            if (request.RegionId <= 0)
            {
                Console.WriteLine($"Invalid RegionId provided: {request.RegionId}");
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "معرف المنطقة غير صالح",
                        English = "Invalid region ID",
                    }
                );
            }

            // Debug log
            Console.WriteLine(
                $"Received request with CategoryId: {request.CategoryId} and RegionId: {request.RegionId}"
            );
            Console.WriteLine($"Number of images: {request.Images?.Count ?? 0}");

            // Validate category exists - with explicit query logging
            var categoryId = request.CategoryId;
            var category = await _context
                .Set<Category>()
                .Where(c => !c.IsDeleted && c.Id == categoryId)
                .FirstOrDefaultAsync(); // Get the full entity

            if (category == null)
            {
                Console.WriteLine($"Category not found in database. Requested ID: {categoryId}");
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الفئة غير موجودة",
                        English = "Category not found",
                    }
                );
            }

            // Validate region exists
            var regionId = request.RegionId;
            var region = await _context
                .Set<Region>()
                .Where(r => !r.IsDeleted && r.Id == regionId)
                .FirstOrDefaultAsync();

            if (region == null)
            {
                Console.WriteLine($"Region not found in database. Requested ID: {regionId}");
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "المنطقة غير موجودة",
                        English = "Region not found",
                    }
                );
            }

            // Debug log
            Console.WriteLine(
                $"Found category - ID: {category.Id}, Name AR: {category.NameAr}, Name EN: {category.NameEn}"
            );
            Console.WriteLine(
                $"Found region - ID: {region.Id}, Name AR: {region.NameArabic}, Name EN: {region.NameEnglish}"
            );

            // Validate at least one image is provided
            if (request.Images == null || !request.Images.Any())
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "يجب تقديم صورة واحدة على الأقل",
                        English = "At least one image must be provided",
                    }
                );
            }

            var now = DateTime.UtcNow;

            // Create new offer first to get ID
            var offer = new Offer
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = category.Id,
                Category = category,
                RegionId = region.Id,
                Region = region,
                ProviderId = GetUserId(),
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true,
                IsDeleted = false,
            };

            // Save offer to get its ID
            await _context.Set<Offer>().AddAsync(offer);
            await _context.SaveChangesAsync();

            Console.WriteLine(
                $"Created offer - ID: {offer.Id}, CategoryId: {category.Id}, RegionId: {region.Id}"
            );

            // Now save the main image with the offer ID
            try
            {
                var mainImageUrl = await _fileStorageService.SaveFileAsync(
                    request.Images[0],
                    $"offers/{offer.Id}/main"
                );
                // Store the relative path for database
                offer.MainImageUrl = mainImageUrl;

                // Update the offer with the main image URL
                _context.Entry(offer).Property(x => x.MainImageUrl).IsModified = true;
                _context.Entry(offer).Property(x => x.UpdatedAt).IsModified = true;
                await _context.SaveChangesAsync();

                Console.WriteLine($"Saved main image: {mainImageUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save main image: {ex.Message}");
                throw;
            }

            // Process additional images if any
            if (request.Images.Count > 1)
            {
                Console.WriteLine($"Processing {request.Images.Count - 1} additional images");
                var offerImages = new List<OfferImage>();

                for (var i = 1; i < request.Images.Count; i++)
                {
                    var imageUrl = await _fileStorageService.SaveFileAsync(
                        request.Images[i],
                        $"offers/{offer.Id}/additional"
                    );
                    Console.WriteLine($"Saved additional image: {imageUrl}");

                    var offerImage = new OfferImage
                    {
                        OfferId = offer.Id,
                        Offer = offer,
                        ImageUrl = imageUrl,
                        CreatedAt = now,
                        UpdatedAt = now,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    offerImages.Add(offerImage);
                }

                if (offerImages.Any())
                {
                    await _context.Set<OfferImage>().AddRangeAsync(offerImages);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Saved {offerImages.Count} additional images to database");
                }
            }

            await _context.Database.CommitTransactionAsync();

            // Get the complete offer with images - using tracked category and region
            var createdOffer = await _context
                .Set<Offer>()
                .Include(o => o.Category)
                .Include(o => o.Region)
                .Include(o => o.ImagesUrl.Where(i => !i.IsDeleted))
                .Where(o => !o.IsDeleted && o.Id == offer.Id)
                .Select(o => new OfferDetailsDto
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
                    AdditionalImages = o.ImagesUrl.Select(i => i.ImageUrl).ToList(),
                    CreatedAt = o.CreatedAt,
                    IsActive = o.IsActive,
                })
                .FirstOrDefaultAsync();

            if (createdOffer == null)
            {
                throw new Exception($"Failed to retrieve created offer with ID {offer.Id}");
            }

            Console.WriteLine(
                $"Final offer - ID: {createdOffer.Id}, CategoryId: {createdOffer.CategoryId}, CategoryName: {createdOffer.CategoryName}, RegionId: {createdOffer.RegionId}, RegionName: {createdOffer.RegionName}, Images: {1 + createdOffer.AdditionalImages.Count}"
            );

            return Represent(
                createdOffer,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء العرض بنجاح",
                    English = "Offer created successfully",
                }
            );
        }
        catch (Exception ex)
        {
            await _context.Database.RollbackTransactionAsync();

            Console.WriteLine($"Error creating offer: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إنشاء العرض",
                    English = "Failed to create offer",
                },
                ex
            );
        }
    }

    [HttpPost("{offerId}/comments")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> CreateComment(int offerId, [FromBody] CreateCommentDto request)
    {
        try
        {
            // Validate offer exists
            var offer = await _context.Offers.FirstOrDefaultAsync(o =>
                o.Id == offerId && !o.IsDeleted && o.IsActive
            );

            if (offer is null)
            {
                return Represent(
                    false,
                    new LocalizedMessage { Arabic = "العرض غير موجود", English = "Offer not found" }
                );
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            if (user is null)
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
            OfferComment? parentComment = null;

            // If it's a reply, validate parent comment exists
            if (request.ReplyToCommentId.HasValue)
            {
                parentComment = await _context.OfferComments.FirstOrDefaultAsync(c =>
                    c.Id == request.ReplyToCommentId.Value && c.OfferId == offerId && !c.IsDeleted
                );

                if (parentComment is null)
                {
                    return Represent(
                        false,
                        new LocalizedMessage
                        {
                            Arabic = "التعليق الأصلي غير موجود",
                            English = "Parent comment not found",
                        }
                    );
                }
            }

            var now = DateTime.UtcNow;
            var userId = GetUserId();
            var comment = new OfferComment
            {
                Comment = request.Comment,
                CreatedAt = now,
                UpdatedAt = now,
                Offer = offer,
                User = user,
                ReplyToComment = parentComment,
                IsActive = true,
                IsDeleted = false,
            };

            _context.OfferComments.Add(comment);
            await _context.SaveChangesAsync();

            var createdComment = await _context
                .OfferComments.Include(c => c.User)
                .Where(c => c.Id == comment.Id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    OfferId = c.OfferId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    UserProfilePhotoUrl = c.User.ProfilePhotoUrl,
                    ReplyToCommentId = c.ReplyToCommentId,
                    CreatedAt = c.CreatedAt,
                    RepliesCount = c.ChildrenComments.Count(),
                })
                .FirstOrDefaultAsync();

            return Represent(
                createdComment,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إضافة التعليق بنجاح",
                    English = "Comment added successfully",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating comment: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إضافة التعليق",
                    English = "Failed to add comment",
                },
                ex
            );
        }
    }

    [HttpGet("{offerId}/comments")]
    public async Task<IActionResult> GetOfferComments(
        int offerId,
        [FromQuery] GetCommentsRequestDto request
    )
    {
        try
        {
            // Check if offer exists
            var offerExists = await _context
                .Set<Offer>()
                .AnyAsync(o => o.Id == offerId && !o.IsDeleted && o.IsActive);

            if (!offerExists)
            {
                return Represent(
                    false,
                    new LocalizedMessage { Arabic = "العرض غير موجود", English = "Offer not found" }
                );
            }

            // Default limit if not provided or invalid
            var limit = request.Limit <= 0 ? 10 : Math.Min(request.Limit, 50);

            // Query root level comments only (not replies)
            var query = _context
                .Set<OfferComment>()
                .Include(c => c.User)
                .Where(c => c.OfferId == offerId && !c.IsDeleted && c.ReplyToCommentId == null);

            // Apply cursor pagination
            if (request.Cursor.HasValue)
            {
                query = request.SortDescending
                    ? query.Where(c => c.Id < request.Cursor) // Get items before cursor for descending order
                    : query.Where(c => c.Id > request.Cursor); // Get items after cursor for ascending order
            }

            // Apply ordering
            query = request.SortDescending
                ? query.OrderByDescending(c => c.Id)
                : query.OrderBy(c => c.Id);

            // Execute query with limit
            var comments = await query
                .Take(limit + 1) // Get one extra item to determine if there are more results
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    OfferId = c.OfferId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    UserProfilePhotoUrl = c.User.ProfilePhotoUrl,
                    ReplyToCommentId = c.ReplyToCommentId,
                    CreatedAt = c.CreatedAt,
                    RepliesCount = c.ChildrenComments.Count(),
                })
                .ToListAsync();

            // Check if there are more results
            var hasMore = comments.Count > limit;
            if (hasMore)
            {
                comments.RemoveAt(comments.Count - 1); // Remove the extra item
            }

            // Get the next cursor
            int? nextCursor = comments.Count > 0 ? comments[comments.Count - 1].Id : null;

            var response = new GetCommentsResponseDto
            {
                Items = comments,
                HasMore = hasMore,
                NextCursor = nextCursor,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم استرجاع التعليقات بنجاح",
                    English = "Comments retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving comments: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في استرجاع التعليقات",
                    English = "Failed to retrieve comments",
                },
                ex
            );
        }
    }

    [HttpGet("comments/{commentId}/replies")]
    public async Task<IActionResult> GetCommentReplies(
        int commentId,
        [FromQuery] GetCommentsRequestDto request
    )
    {
        try
        {
            // Check if parent comment exists
            var parentComment = await _context
                .Set<OfferComment>()
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            if (parentComment == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "التعليق غير موجود",
                        English = "Comment not found",
                    }
                );
            }

            // Default limit if not provided or invalid
            var limit = request.Limit <= 0 ? 10 : Math.Min(request.Limit, 50);

            // Query replies to the specified comment
            var query = _context
                .Set<OfferComment>()
                .Include(c => c.User)
                .Where(c => c.ReplyToCommentId == commentId && !c.IsDeleted);

            // Apply cursor pagination
            if (request.Cursor.HasValue)
            {
                query = request.SortDescending
                    ? query.Where(c => c.Id < request.Cursor) // Get items before cursor for descending order
                    : query.Where(c => c.Id > request.Cursor); // Get items after cursor for ascending order
            }

            // Apply ordering
            query = request.SortDescending
                ? query.OrderByDescending(c => c.Id)
                : query.OrderBy(c => c.Id);

            // Execute query with limit
            var replies = await query
                .Take(limit + 1) // Get one extra item to determine if there are more results
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    OfferId = c.OfferId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    UserProfilePhotoUrl = c.User.ProfilePhotoUrl,
                    ReplyToCommentId = c.ReplyToCommentId,
                    CreatedAt = c.CreatedAt,
                    RepliesCount = 0, // No nested replies, so always 0
                })
                .ToListAsync();

            // Check if there are more results
            var hasMore = replies.Count > limit;
            if (hasMore)
            {
                replies.RemoveAt(replies.Count - 1); // Remove the extra item
            }

            // Get the next cursor
            int? nextCursor = replies.Count > 0 ? replies[replies.Count - 1].Id : null;

            var response = new GetCommentsResponseDto
            {
                Items = replies,
                HasMore = hasMore,
                NextCursor = nextCursor,
            };

            return Represent(
                response,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم استرجاع الردود بنجاح",
                    English = "Replies retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving replies: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في استرجاع الردود",
                    English = "Failed to retrieve replies",
                },
                ex
            );
        }
    }

    [HttpPost("{offerId}/favorite")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> ToggleOfferFavorite(int offerId)
    {
        try
        {
            var userId = GetUserId();

            // Check if the offer exists
            var offer = await _context.Offers.FirstOrDefaultAsync(o =>
                o.Id == offerId && !o.IsDeleted && o.IsActive
            );

            if (offer == null)
            {
                return Represent(
                    false,
                    new LocalizedMessage { Arabic = "العرض غير موجود", English = "Offer not found" }
                );
            }

            // Check if the offer is already in favorites
            var existingFavorite = await _context.Favorites.FirstOrDefaultAsync(f =>
                f.UserId == userId && f.OfferId == offerId && !f.IsDeleted
            );

            if (existingFavorite != null)
            {
                // If it exists, remove from favorites
                _context.Favorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();

                return Represent(
                    true,
                    new LocalizedMessage
                    {
                        Arabic = "تم إزالة العرض من المفضلة",
                        English = "Offer removed from favorites",
                    }
                );
            }
            else
            {
                // If it doesn't exist, add to favorites
                var now = DateTime.UtcNow;
                var favorite = new Favorite
                {
                    UserId = userId,
                    OfferId = offerId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsActive = true,
                    IsDeleted = false,
                };

                await _context.Favorites.AddAsync(favorite);
                await _context.SaveChangesAsync();

                return Represent(
                    true,
                    new LocalizedMessage
                    {
                        Arabic = "تم إضافة العرض إلى المفضلة",
                        English = "Offer added to favorites",
                    }
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling favorite: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في تحديث المفضلة",
                    English = "Failed to update favorites",
                },
                ex
            );
        }
    }
}

public class CreateOfferDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public int CategoryId { get; set; }
    public int RegionId { get; set; }
    public List<IFormFile> Images { get; set; } = [];
}

public class GetOffersRequestDto
{
    public int? Cursor { get; set; }
    public int Limit { get; set; } = 10;
    public bool SortDescending { get; set; } = true;
    public int CategoryId { get; set; }
    public int RegionId { get; set; }
    public double MinPrice { get; set; }
    public double MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetOffersResponseDto
{
    public List<OfferPaginatedItemDto> Items { get; set; } = new();
    public bool HasMore { get; set; }
    public int? NextCursor { get; set; }
}

public class OfferPaginatedItemDto
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
    public int NumberOfFavorites { get; set; }
    public int NumberOfViews { get; set; }
    public int NumberOfComments { get; set; }
}

public class OfferDetailsPageDto
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
    public List<string> AdditionalImages { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderPhoneNumber { get; set; } = string.Empty;
    public string? ProviderProfilePhotoUrl { get; set; } = string.Empty;
    public string ProviderCreatedAt { get; set; }
    public bool IsFavorite { get; set; }
    public int NumberOfFavorites { get; set; }
    public int NumberOfViews { get; set; }
}

public class CreateCommentDto
{
    public string Comment { get; set; } = string.Empty;
    public int? ReplyToCommentId { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int OfferId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserProfilePhotoUrl { get; set; }
    public int? ReplyToCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RepliesCount { get; set; }
}

public class GetCommentsRequestDto
{
    public int? Cursor { get; set; }
    public int Limit { get; set; } = 10;
    public bool SortDescending { get; set; } = true;
}

public class GetCommentsResponseDto
{
    public List<CommentDto> Items { get; set; } = new();
    public bool HasMore { get; set; }
    public int? NextCursor { get; set; }
}
