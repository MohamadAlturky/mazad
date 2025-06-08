using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Categories.Create;

public class CreateCategoryCommand : BaseCommand
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}

public class CreateCategoryApiRequest : BaseApiRequest<CreateCategoryCommand>
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public int? ParentId { get; set; }

    public override CreateCategoryCommand ToCommand(int userId, string language)
    {
        return new CreateCategoryCommand
        {
            NameArabic = NameArabic,
            NameEnglish = NameEnglish,
            ParentId = ParentId,
            UserId = userId,
            Language = language
        };
    }
}
