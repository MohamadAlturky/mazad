using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Create;

public class CreateDynamicAttributeCommandValidator : BaseCommandValidator<CreateDynamicAttributeCommand>
{
    public override Result Validate(CreateDynamicAttributeCommand command)
    {
        // Validate that NameArabic is not empty or whitespace
        if (string.IsNullOrWhiteSpace(command.NameArabic))
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "اسم السمة باللغة العربية مطلوب.",
                English = "Attribute Arabic name is required."
            });
        }

        // Validate that NameEnglish is not empty or whitespace
        if (string.IsNullOrWhiteSpace(command.NameEnglish))
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "اسم السمة باللغة الإنجليزية مطلوب.",
                English = "Attribute English name is required."
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