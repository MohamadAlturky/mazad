using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Toggle;

public class ToggleDynamicAttributeCommandValidator : BaseCommandValidator<ToggleDynamicAttributeCommand>
{
    public override Result Validate(ToggleDynamicAttributeCommand command)
    {
        // Validate that Id is greater than 0
        if (command.Id <= 0)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "معرف السمة غير صالح.",
                English = "Invalid attribute ID."
            });
        }

        // If all validations pass, return a success result
        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم التحقق من صحة الأمر بنجاح.",
            English = "Command validated successfully."
        });
    }
} 