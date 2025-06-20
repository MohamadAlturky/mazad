using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Users.Register;

public class RegisterUserCommandValidator
    : BaseCommandValidator<RegisterUserCommand, RegisterUserResponse>
{
    public override Result<RegisterUserResponse> Validate(RegisterUserCommand command)
    {
        // Validate that Name is not empty or whitespace
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<RegisterUserResponse>.Fail(
                new LocalizedMessage { Arabic = "الاسم مطلوب.", English = "Name is required." }
            );
        }

        // Validate that Phone is not empty or whitespace
        if (string.IsNullOrWhiteSpace(command.Phone))
        {
            return Result<RegisterUserResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "رقم الهاتف مطلوب.",
                    English = "Phone number is required.",
                }
            );
        }
        // Basic phone number format validation (you might want a more robust regex)
        else if (!System.Text.RegularExpressions.Regex.IsMatch(command.Phone, @"^\d{10,15}$"))
        {
            return Result<RegisterUserResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "صيغة رقم الهاتف غير صالحة.",
                    English = "Invalid phone number format.",
                }
            );
        }

        // If all validations pass, return a success result
        return Result<RegisterUserResponse>.Ok(
            new RegisterUserResponse
            {
                UserId = 0,
                Name = command.Name,
                Phone = command.Phone,
            },
            new LocalizedMessage
            {
                Arabic = "تم التحقق من صحة أمر التسجيل بنجاح.",
                English = "Registration command validated successfully.",
            }
        );
    }
}
