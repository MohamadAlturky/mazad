using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Read;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Read;

public class GetDynamicAttributesByCategoryIdQuery : BaseQuery<List<CategoryDynamicAttributeDto>>
{
    public int CategoryId { get; set; }
}

public class GetDynamicAttributesByCategoryIdQueryHandler
    : BaseQueryHandler<GetDynamicAttributesByCategoryIdQuery, List<CategoryDynamicAttributeDto>>
{
    private readonly MazadDbContext _context;

    public GetDynamicAttributesByCategoryIdQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<List<CategoryDynamicAttributeDto>>> Handle(GetDynamicAttributesByCategoryIdQuery query)
    {

        var categoryAttributes = await _context.CategoryAttributes
            .AsNoTracking()
            .Where(ca => ca.CategoryId == query.CategoryId)
            .Select(ca => new DynamicAttributeDbDto
            {
                Id = ca.DynamicAttributeId,
                Name = query.Language == "ar" ? ca.DynamicAttribute.NameArabic : ca.DynamicAttribute.NameEnglish,
                IsActive = ca.DynamicAttribute.IsActive,
                AttributeValueType = ca.DynamicAttribute.AttributeValueType,
            })
            .ToListAsync();

        var allAttributes = await _context.DynamicAttributes
            .AsNoTracking()
            .Select(a => new DynamicAttributeDbDto
            {
                Id = a.Id,
                Name = query.Language == "ar" ? a.NameArabic : a.NameEnglish,
                IsActive = a.IsActive,
                AttributeValueType = a.AttributeValueType,
            })
            .ToListAsync();
        var unSelectedAttributes = allAttributes.Where(a => categoryAttributes.Any(ca => ca.Id == a.Id)).ToList();
        var selectedAttributes = allAttributes.Where(a => !categoryAttributes.Any(ca => ca.Id == a.Id)).ToList();

        List<CategoryDynamicAttributeDto> reponse = new List<CategoryDynamicAttributeDto>();

        foreach (var attribute in unSelectedAttributes)
        {
            reponse.Add(new CategoryDynamicAttributeDto { Id = attribute.Id, Name = attribute.Name, IsActive = attribute.IsActive, AttributeValueTypeString = DynamicAttributeHelper.RepresentAttributeValueType(attribute.AttributeValueType, query.Language), IsSelected = true });
        }

        foreach (var attribute in selectedAttributes)
        {
            reponse.Add(new CategoryDynamicAttributeDto { Id = attribute.Id, Name = attribute.Name, IsActive = attribute.IsActive, AttributeValueTypeString = DynamicAttributeHelper.RepresentAttributeValueType(attribute.AttributeValueType, query.Language), IsSelected = false });
        }

        return Result<List<CategoryDynamicAttributeDto>>.Ok(reponse, new LocalizedMessage
            {
                Arabic = "تم الحصول على جميع السمات بنجاح",
                English = "All attributes retrieved successfully"
            });
    }
}

public class CategoryDynamicAttributeDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
    public required string AttributeValueTypeString { get; set; }
    public required bool IsSelected { get; set; }
}