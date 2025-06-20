using Mazad.Core.Shared.Results;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Api.Controllers;

[ApiController]
[Route("api/otp")]
public class OtpController : BaseController
{
    private readonly IOtpService _otpService;

    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateOtpResponse), 200)]
    [ProducesResponseType(typeof(GenerateOtpResponse), 400)]
    public IActionResult Generate([FromBody] GenerateOtpRequest request)
    {
        var response = _otpService.GenerateOtp(request);
        if (!response.Success)
        {
            return Represent(
                Result<GenerateOtpResponse>.Fail(
                    new LocalizedMessage
                    {
                        Arabic = "خطأ في إنشاء رمز التحقق.",
                        English = "Error generating OTP.",
                    }
                )
            );
        }
        return Represent(
            Result<GenerateOtpResponse>.Ok(
                response,
                new LocalizedMessage
                {
                    Arabic = "تم إنشاء رمز التحقق بنجاح.",
                    English = "OTP generated successfully.",
                }
            )
        );
    }

    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateOtpResponse), 200)]
    [ProducesResponseType(typeof(ValidateOtpResponse), 400)]
    public IActionResult Validate([FromBody] ValidateOtpRequest request)
    {
        var response = _otpService.ValidateOtp(request);
        if (!response.IsValid)
        {
            return Represent(
                Result<ValidateOtpResponse>.Fail(
                    new LocalizedMessage
                    {
                        Arabic = "خطأ في التحقق من رمز التحقق.",
                        English = "Error validating OTP.",
                    }
                )
            );
        }
        return Represent(
            Result<ValidateOtpResponse>.Ok(
                response,
                new LocalizedMessage
                {
                    Arabic = "تم التحقق من رمز التحقق بنجاح.",
                    English = "OTP validated successfully.",
                }
            )
        );
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(GetAllOtpsResponse), 200)]
    public IActionResult GetAllOtps()
    {
        var response = _otpService.GetAllOtps();
        return Represent(
            Result<GetAllOtpsResponse>.Ok(
                response,
                new LocalizedMessage
                {
                    Arabic = "تم الحصول على جميع الرموز المرورية بنجاح.",
                    English = "All OTPs retrieved successfully.",
                }
            )
        );
    }
}
