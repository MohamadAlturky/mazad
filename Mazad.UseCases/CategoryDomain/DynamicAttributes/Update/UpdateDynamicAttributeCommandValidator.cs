using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Update;

public class UpdateDynamicAttributeCommandValidator : BaseCommandValidator<UpdateDynamicAttributeCommand>
{
    public override Result Validate(UpdateDynamicAttributeCommand command)
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

        if (command.NameArabic is not null)
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
        }

        if (command.NameEnglish is not null)
        {
            // Validate that NameEnglish is not empty or whitespace
            if (string.IsNullOrWhiteSpace(command.NameEnglish))
            {
                return Result.Fail(new LocalizedMessage
                {
                    Arabic = "اسم السمة باللغة الإنجليزية مطلوب.",
                    English = "Attribute English name is required."
                });
            }
        }

        // If all validations pass, return a success result
        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم التحقق من صحة الأمر بنجاح.",
            English = "Command validated successfully."
        });
    }
}