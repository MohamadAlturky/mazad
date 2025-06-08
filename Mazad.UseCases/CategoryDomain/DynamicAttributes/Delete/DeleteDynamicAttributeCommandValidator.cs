using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Delete;

public class DeleteDynamicAttributeCommandValidator : BaseCommandValidator<DeleteDynamicAttributeCommand>
{
    public override Result Validate(DeleteDynamicAttributeCommand command)
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