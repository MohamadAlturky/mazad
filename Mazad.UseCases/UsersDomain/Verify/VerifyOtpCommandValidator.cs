using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Users.VerifyOtp;

public class VerifyOtpCommandValidator : BaseCommandValidator<VerifyOtpCommand, VerifyOtpResponse>
{
    public override Result<VerifyOtpResponse> Validate(VerifyOtpCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.UserIdToVerify))
        {
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "معرف المستخدم مطلوب.",
                    English = "User ID is required.",
                }
            );
        }

        if (string.IsNullOrWhiteSpace(command.Otp))
        {
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage { Arabic = "رمز التحقق مطلوب.", English = "OTP is required." }
            );
        }

        // Basic OTP format validation (assuming it's numeric and has a typical length)
        // You might want to make the length configurable via settings.
        if (!System.Text.RegularExpressions.Regex.IsMatch(command.Otp, @"^\d{4,6}$")) // Assuming 4-6 digit OTP
        {
            return Result<VerifyOtpResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "صيغة رمز التحقق غير صالحة.",
                    English = "Invalid OTP format.",
                }
            );
        }

        return Result<VerifyOtpResponse>.Ok(
            new VerifyOtpResponse
            {
                Token = string.Empty,
                Name = string.Empty,
                Phone = string.Empty,
            },
            new LocalizedMessage
            {
                Arabic = "تم التحقق من صحة أمر التحقق من رمز التحقق بنجاح.",
                English = "Verify OTP command validated successfully.",
            }
        );
    }
}
