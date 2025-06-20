using System;
using System.Threading.Tasks;
using Mazad.Core.Domain.Users; // Assuming User entity is in this namespace
using Mazad.Core.Domain.Users.Authentication;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.EntityFrameworkCore; // For AnyAsync

namespace Mazad.UseCases.Users.VerifyOtp;

public class VerifyOtpCommandHandler : BaseCommandHandler<VerifyOtpCommand, VerifyOtpResponse>
{
    private readonly MazadDbContext _context;
    private readonly VerifyOtpCommandValidator _validator;
    private readonly IOtpService _otpService;
    private readonly JwtService _jwtService;

    public VerifyOtpCommandHandler(
        MazadDbContext context,
        IOtpService otpService,
        JwtService jwtService
    )
    {
        _context = context;
        _validator = new VerifyOtpCommandValidator();
        _otpService = otpService;
        _jwtService = jwtService;
    }

    public override async Task<Result<VerifyOtpResponse>> Handle(VerifyOtpCommand command)
    {
        // 1. Validate the command
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        // 2. Validate the OTP using the OtpService
        var otpValidationResponse = _otpService.ValidateOtp(
            new ValidateOtpRequest { UserId = command.UserIdToVerify, Otp = command.Otp }
        );

        if (!otpValidationResponse.IsValid)
        {
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = otpValidationResponse.Message, // Use message from OTP service
                    English = otpValidationResponse.Message, // Use message from OTP service
                }
            );
        }

        // 3. Find the user and activate their account if OTP is valid
        // Assuming UserId in OtpRecord is the actual User.Id from your database
        if (!int.TryParse(command.UserIdToVerify, out int userId))
        {
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "صيغة معرف المستخدم غير صالحة.",
                    English = "Invalid User ID format.",
                }
            );
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage { Arabic = "المستخدم غير موجود.", English = "User not found." }
            );
        }
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error activating user: {ex.Message}");
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "حدث خطأ أثناء تفعيل حساب المستخدم.",
                    English = "An error occurred while activating the user account.",
                },
                ex
            );
        }
        var token = _jwtService.GenerateToken(user.Id, user.Name);

        // 4. Return success
        return Result<VerifyOtpResponse>.Ok(
            new VerifyOtpResponse
            {
                Name = user.Name,
                Phone = user.Phone,
                Token = token,
            },
            new LocalizedMessage
            {
                Arabic = "تم التحقق من رمز التحقق وتفعيل حساب المستخدم بنجاح.",
                English = "OTP verified and user account activated successfully.",
            }
        );
    }
}
