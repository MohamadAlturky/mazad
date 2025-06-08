using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.DynamicAttributes.Read;

public class GetAllDynamicAttributesQuery : BaseQuery<List<DynamicAttributeDto>>
{
}

public class GetAllDynamicAttributesQueryHandler : BaseQueryHandler<GetAllDynamicAttributesQuery, List<DynamicAttributeDto>>
{
    private readonly MazadDbContext _context;

    public GetAllDynamicAttributesQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<List<DynamicAttributeDto>>> Handle(GetAllDynamicAttributesQuery query)
    {
        var attributes = await _context.DynamicAttributes
            .AsNoTracking() // Use AsNoTracking for read-only operations for performance
            .Select(a => new DynamicAttributeDto
            {
                Id = a.Id,
                Name = query.Language == "ar" ? a.NameArabic : a.NameEnglish,
                IsActive = a.IsActive,
                AttributeValueType = a.AttributeValueType,
                AttributeValueTypeString = RepresentAttributeValueType(a.AttributeValueType, query.Language)
            })
            .ToListAsync();

        return Result<List<DynamicAttributeDto>>.Ok(attributes, new LocalizedMessage
        {
            Arabic = "تم الحصول على جميع السمات بنجاح",
            English = "All attributes retrieved successfully"
        });
    }

    private string RepresentAttributeValueType(AttributeValueType attributeValueType, string language)
    {
        if (language == "ar")
        {
            return attributeValueType switch
            {
                AttributeValueType.String => "نص",
                AttributeValueType.Number => "رقم",
                AttributeValueType.Boolean => "صحيح/خطأ",
                _ => "Unknown"
            };
        }
        else
        {
            return attributeValueType switch
            {
                AttributeValueType.String => "String",
                AttributeValueType.Number => "Number",
                AttributeValueType.Boolean => "True/False",
                _ => "Unknown"
            };
        }
    }
}

public class DynamicAttributeDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
    public required AttributeValueType AttributeValueType { get; set; }
    public required string AttributeValueTypeString { get; set; }
}
