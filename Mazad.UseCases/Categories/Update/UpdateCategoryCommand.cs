using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Categories.Update;

public class UpdateCategoryCommand : BaseCommand
{
    public int Id { get; set; }
    public string? NameArabic { get; set; }
    public string? NameEnglish { get; set; }
    public int? ParentId { get; set; }
}

public class UpdateCategoryApiRequest : BaseApiRequest<UpdateCategoryCommand>
{
    public int Id { get; set; }
    public string? NameArabic { get; set; }
    public string? NameEnglish { get; set; }
    public int? ParentId { get; set; }

    public override UpdateCategoryCommand ToCommand(int userId, string language)
    {
        return new UpdateCategoryCommand
        {
            Id = Id,
            NameArabic = NameArabic,
            NameEnglish = NameEnglish,
            ParentId = ParentId,
            UserId = userId,
            Language = language
        };
    }
}