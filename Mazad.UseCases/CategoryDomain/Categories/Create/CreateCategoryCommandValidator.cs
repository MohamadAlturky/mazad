using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Categories.Create;

public class CreateCategoryCommandValidator : BaseCommandValidator<CreateCategoryCommand>
{
    public override Result Validate(CreateCategoryCommand command)
    {
        // Validate that NameArabic is not empty or whitespace
        if (string.IsNullOrWhiteSpace(command.NameArabic))
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "اسم الفئة باللغة العربية مطلوب.",
                English = "Category Arabic name is required."
            });
        }

        // Validate that NameEnglish is not empty or whitespace
        if (string.IsNullOrWhiteSpace(command.NameEnglish))
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "اسم الفئة باللغة الإنجليزية مطلوب.",
                English = "Category English name is required."
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
