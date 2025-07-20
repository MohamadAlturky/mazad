using Mazad.Api.Controllers;
using Mazad.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Controllers.Shared;

[ApiController]
[Route("api/[controller]")]
public class SharedFileController : BaseController
{
    private readonly IFileStorageService _fileStorageService;

    public SharedFileController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(
        IFormFile file,
        [FromQuery] string directory = "images"
    )
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "لم يتم تحديد ملف",
                        English = "No file was selected",
                    }
                );
            }

            // Validate file size (e.g., 5MB max)
            if (file.Length > 5 * 1024 * 1024)
            {
                return Represent(
                    false,
                    new LocalizedMessage
                    {
                        Arabic = "حجم الملف كبير جداً",
                        English = "File size is too large",
                    }
                );
            }

            var filePath = await _fileStorageService.SaveFileAsync(file, directory);

            return Represent(
                new { url = $"/api/sharedfile/get/{filePath}" },
                true,
                new LocalizedMessage
                {
                    Arabic = "تم رفع الملف بنجاح",
                    English = "File uploaded successfully",
                }
            );
        }
        catch (ArgumentException ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "نوع الملف غير مسموح به",
                    English = "File type not allowed",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في رفع الملف",
                    English = "Failed to upload file",
                },
                ex
            );
        }
    }

    // [HttpGet("get/{**filePath}")]
    // public async Task<IActionResult> GetFile(string filePath)
    // {
    //     try
    //     {
    //         // URL decode the path and normalize slashes
    //         filePath = Uri.UnescapeDataString(filePath).Replace("\\", "/").TrimStart('/');

    //         Console.WriteLine($"Attempting to get file: {filePath}");

    //         var (fileContents, contentType) = await _fileStorageService.GetFileAsync(filePath);
    //         return File(fileContents, contentType);
    //     }
    //     catch (FileNotFoundException)
    //     {
    //         Console.WriteLine($"File not found: {filePath}");
    //         return NotFound(
    //             new
    //             {
    //                 Success = false,
    //                 Message = new LocalizedMessage
    //                 {
    //                     Arabic = "الملف غير موجود",
    //                     English = "File not found",
    //                 },
    //             }
    //         );
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error retrieving file {filePath}: {ex.Message}");
    //         return Represent(
    //             false,
    //             new LocalizedMessage
    //             {
    //                 Arabic = "فشل في جلب الملف",
    //                 English = "Failed to retrieve file",
    //             },
    //             ex
    //         );
    //     }
    // }
}
