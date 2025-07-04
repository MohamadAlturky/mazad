using Mazad.Api.Controllers;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : BaseController
{
    private readonly IOtpService _otpService;

    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOtps()
    {
        try
        {
            var otps = await _otpService.GetAllOtps();

            return Represent(
                otps,
                true,
                new LocalizedMessage
                {
                    Arabic = "تم جلب رموز التحقق بنجاح",
                    English = "OTPs retrieved successfully",
                }
            );
        }
        catch (Exception ex)
        {
            return Represent(
                false,
                new LocalizedMessage
                {
                    Arabic = "فشل في جلب رموز التحقق",
                    English = "Failed to retrieve OTPs",
                },
                ex
            );
        }
    }
}
