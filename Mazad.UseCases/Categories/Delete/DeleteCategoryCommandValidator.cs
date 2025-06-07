using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Categories.Delete;

public class DeleteCategoryCommandValidator : BaseCommandValidator<DeleteCategoryCommand>
{
    public override Result Validate(DeleteCategoryCommand command)
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

        // If all validations pass, return a success result
        return Result.Ok(new LocalizedMessage
        {
            Arabic = "تم التحقق من صحة الأمر بنجاح.",
            English = "Command validated successfully."
        });
    }
}