using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Create;

public class CreateCategoryAttributeCommandValidator : BaseCommandValidator<CreateCategoryAttributeCommand>
{
    public override Result Validate(CreateCategoryAttributeCommand command)
    {
        // Validate that CategoryId is greater than 0
        if (command.CategoryId <= 0)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "معرف الفئة غير صالح.",
                English = "Invalid category ID."
            });
        }

        // Validate that DynamicAttributeId is greater than 0
        if (command.DynamicAttributeId <= 0)
        {
            return Result.Fail(new LocalizedMessage
            {
                Arabic = "معرف السمة غير صالح.",
                English = "Invalid dynamic attribute ID."
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