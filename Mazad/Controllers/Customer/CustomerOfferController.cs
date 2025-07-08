using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.Interfaces;
using Mazad.Models;
using Mazad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mazad.Core.Domain.Regions;
using Mazad.Controllers;
using Mazad.Core.Shared.OfferDetails;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerOfferController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CustomerOfferController(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    [HttpPost("create")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> CreateOffer([FromForm] CreateOfferDto request)
    {
        await _unitOfWork.BeginTransactionAsync();
        
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
                        English = "Invalid category ID"
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
                        English = "Invalid region ID"
                    }
                );
            }

            // Debug log
            Console.WriteLine($"Received request with CategoryId: {request.CategoryId} and RegionId: {request.RegionId}");
            Console.WriteLine($"Number of images: {request.Images?.Count ?? 0}");

            // Validate category exists - with explicit query logging
            var categoryId = request.CategoryId;
            var category = await _unitOfWork.Context.Set<Category>()
                .Where(c => !c.IsDeleted && c.Id == categoryId)
                .FirstOrDefaultAsync();  // Get the full entity
            
            if (category == null)
            {
                Console.WriteLine($"Category not found in database. Requested ID: {categoryId}");
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "الفئة غير موجودة",
                        English = "Category not found"
                    }
                );
            }

            // Validate region exists
            var regionId = request.RegionId;
            var region = await _unitOfWork.Context.Set<Region>()
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
                        English = "Region not found"
                    }
                );
            }

            // Debug log
            Console.WriteLine($"Found category - ID: {category.Id}, Name AR: {category.NameAr}, Name EN: {category.NameEn}");
            Console.WriteLine($"Found region - ID: {region.Id}, Name AR: {region.NameArabic}, Name EN: {region.NameEnglish}");

            // Validate at least one image is provided
            if (request.Images == null || !request.Images.Any())
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "يجب تقديم صورة واحدة على الأقل",
                        English = "At least one image must be provided"
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
                IsDeleted = false
            };

            // Save offer to get its ID
            await _unitOfWork.Context.Set<Offer>().AddAsync(offer);
            await _unitOfWork.SaveChangesAsync();
            
            Console.WriteLine($"Created offer - ID: {offer.Id}, CategoryId: {category.Id}, RegionId: {region.Id}");

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
                _unitOfWork.Context.Entry(offer).Property(x => x.MainImageUrl).IsModified = true;
                _unitOfWork.Context.Entry(offer).Property(x => x.UpdatedAt).IsModified = true;
                await _unitOfWork.SaveChangesAsync();

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
                        IsDeleted = false
                    };
                    offerImages.Add(offerImage);
                }

                if (offerImages.Any())
                {
                    await _unitOfWork.Context.Set<OfferImage>().AddRangeAsync(offerImages);
                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine($"Saved {offerImages.Count} additional images to database");
                }
            }

            await _unitOfWork.CommitAsync();

            // Get the complete offer with images - using tracked category and region
            var createdOffer = await _unitOfWork.Context.Set<Offer>()
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
                    AdditionalImages = o.ImagesUrl
                        .Select(i => i.ImageUrl)
                        .ToList(),
                    CreatedAt = o.CreatedAt,
                    IsActive = o.IsActive
                })
                .FirstOrDefaultAsync();

            if (createdOffer == null)
            {
                throw new Exception($"Failed to retrieve created offer with ID {offer.Id}");
            }

            Console.WriteLine($"Final offer - ID: {createdOffer.Id}, CategoryId: {createdOffer.CategoryId}, CategoryName: {createdOffer.CategoryName}, RegionId: {createdOffer.RegionId}, RegionName: {createdOffer.RegionName}, Images: {1 + createdOffer.AdditionalImages.Count}");

            return Represent(
                createdOffer,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء العرض بنجاح",
                    English = "Offer created successfully"
                }
            );
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            
            Console.WriteLine($"Error creating offer: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في إنشاء العرض",
                    English = "Failed to create offer"
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
