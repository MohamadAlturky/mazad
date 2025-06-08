using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Categories.Update;

public class UpdateCategoryCommandValidator : BaseCommandValidator<UpdateCategoryCommand>
{
    public override Result Validate(UpdateCategoryCommand command)
    {
        // Validate that Id is provided and is a positive integer
        if (command.Id <= 0)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "معرف الفئة مطلوب وصالح.",
                English = "Category ID is required and must be valid."
            });
        }

        // Validate that NameArabic is not empty or whitespace
        if (command.NameArabic is not null)
        {
            if (string.IsNullOrWhiteSpace(command.NameArabic))
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "اسم الفئة باللغة العربية مطلوب.",
                English = "Category Arabic name is required."
            });
        }
        }

        // Validate that NameEnglish is not empty or whitespace
        if (command.NameEnglish is not null)
        {
            if (string.IsNullOrWhiteSpace(command.NameEnglish))
            {
                return Result.Fail(new LocalizedMessage
                {
                    Arabic = "اسم الفئة باللغة الإنجليزية مطلوب.",
                    English = "Category English name is required."
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