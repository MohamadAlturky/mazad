using Mazad.Core.Domain.Users; // Assuming User entity is here
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Users.Login;

public class LoginCommandHandler : BaseCommandHandler<LoginCommand, LoginResponse>
{
    private readonly MazadDbContext _context;
    private readonly LoginCommandValidator _validator;
    private readonly IOtpService _otpService;

    public LoginCommandHandler(MazadDbContext context, IOtpService otpService)
    {
        _context = context;
        _validator = new LoginCommandValidator();
        _otpService = otpService;
    }

    public override async Task<Result<LoginResponse>> Handle(LoginCommand command)
    {
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == command.Phone);

        if (user == null)
        {
            return Result<LoginResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "رقم الهاتف غير موجود.",
                    English = "Phone number not found.",
                }
            );
        }

        if (!user.IsActive)
        {
            return Result<LoginResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "الحساب غير مفعل. يرجى تفعيل حسابك أولاً.",
                    English = "Account not activated. Please activate your account first.",
                }
            );
        }
        var otpResponse = _otpService.GenerateOtp(
            new GenerateOtpRequest { UserId = user.Id.ToString() }
        );
        if (!otpResponse.Success)
        {
            return Result<LoginResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "خطأ في إنشاء رمز التحقق.",
                    English = "Error generating OTP.",
                }
            );
        }
        return Result<LoginResponse>.Ok(
            new LoginResponse
            {
                Name = user.Name,
                Phone = user.Phone,
                UserId = user.Id,
            },
            new LocalizedMessage
            {
                Arabic = "تم تسجيل الدخول بنجاح.",
                English = "Login successful.",
            }
        );
    }
}
