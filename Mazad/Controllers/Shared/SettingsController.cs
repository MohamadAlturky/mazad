using Mazad.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Controllers.Shared;

[ApiController]
[Route("api/settings")]
public class SettingsController : BaseController
{
    public SettingsController()
    {
    }

    [HttpGet("app-download-links")]
    public IActionResult GetAppDownloadLinks()
    {
        try
        {
            var downloadLinks = new AppDownloadLinksDto
            {
                GooglePlayUrl = "https://play.google.com/store/apps/details?id=com.mazad.app",
                AppleAppStoreUrl = "https://apps.apple.com/app/mazad/id1234567890"
            };

            return Represent(
                downloadLinks,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب روابط تحميل التطبيق بنجاح",
                    English = "App download links retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب روابط تحميل التطبيق",
                    English = "Failed to retrieve app download links",
                },
                ex
            );
        }
    }
}

public class AppDownloadLinksDto
{
    public string GooglePlayUrl { get; set; } = string.Empty;
    public string AppleAppStoreUrl { get; set; } = string.Empty;
}
