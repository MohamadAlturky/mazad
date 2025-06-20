using System.Text.RegularExpressions;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Users.Login;

public class LoginCommandValidator : BaseCommandValidator<LoginCommand, LoginResponse>
{
    public override Result<LoginResponse> Validate(LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Phone))
        {
            return Result<LoginResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "رقم الهاتف مطلوب.",
                    English = "Phone number is required.",
                }
            );
        }
        else if (!Regex.IsMatch(command.Phone, @"^\d{10,15}$"))
        {
            return Result<LoginResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "صيغة رقم الهاتف غير صالحة.",
                    English = "Invalid phone number format.",
                }
            );
        }
        return Result<LoginResponse>.Ok(
            new LoginResponse
            {
                Name = string.Empty,
                Phone = command.Phone,
                UserId = 0,
            },
            new LocalizedMessage
            {
                Arabic = "تم التحقق من صحة أمر تسجيل الدخول بنجاح.",
                English = "Login command validated successfully.",
            }
        );
    }
}
