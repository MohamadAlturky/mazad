using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Read;

public class GetDynamicAttributesBasicInfoQuery : BaseQuery<DynamicAttributeBasicInfoDto>
{
    public required int Id { get; set; }
}

public class GetDynamicAttributesBasicInfoQueryHandler : BaseQueryHandler<GetDynamicAttributesBasicInfoQuery, DynamicAttributeBasicInfoDto>
{
    private readonly MazadDbContext _context;

    public GetDynamicAttributesBasicInfoQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<DynamicAttributeBasicInfoDto>> Handle(GetDynamicAttributesBasicInfoQuery query)
    {
        var attributes = await _context.DynamicAttributes
            .Where(a => a.Id == query.Id)
            .AsNoTracking()
            .Select(a => new DynamicAttributeBasicInfoDto
            {
                Id = a.Id,
                NameEnglish = a.NameEnglish,
                NameArabic = a.NameArabic,
                AttributeValueType = a.AttributeValueType
            })
            .FirstOrDefaultAsync();

        return Result<DynamicAttributeBasicInfoDto>.Ok(attributes, new LocalizedMessage
        {
            Arabic = "تم الحصول على معلومات السمات بنجاح",
            English = "Attributes information retrieved successfully"
        });
    }
}

public class DynamicAttributeBasicInfoDto
{
    public required int Id { get; set; }
    public required string NameEnglish { get; set; }
    public required string NameArabic { get; set; }
    public required AttributeValueType AttributeValueType { get; set; }
}