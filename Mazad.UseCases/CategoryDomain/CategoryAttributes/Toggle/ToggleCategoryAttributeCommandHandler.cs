using Mazad.Core.Domain.Categories;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Mazad.UseCases.CategoryDomain.CategoryAttributes.Create;
using Mazad.UseCases.CategoryDomain.CategoryAttributes.Delete;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.CategoryDomain.CategoryAttributes.Toggle;

public class ToggleCategoryAttributeCommandHandler : BaseCommandHandler<ToggleCategoryAttributeCommand>
{
    private readonly MazadDbContext _context;
    private readonly ToggleCategoryAttributeCommandValidator _validator;
    private readonly DeleteCategoryAttributeCommandHandler _deleteCategoryAttributeCommandHandler;
    private readonly CreateCategoryAttributeCommandHandler _createCategoryAttributeCommandHandler;

    public ToggleCategoryAttributeCommandHandler(
        MazadDbContext context,
        DeleteCategoryAttributeCommandHandler deleteCategoryAttributeCommandHandler,
        CreateCategoryAttributeCommandHandler createCategoryAttributeCommandHandler)
    {
        _context = context;
        _validator = new ToggleCategoryAttributeCommandValidator();
        _deleteCategoryAttributeCommandHandler = deleteCategoryAttributeCommandHandler;
        _createCategoryAttributeCommandHandler = createCategoryAttributeCommandHandler;
    }

    public override async Task<Result> Handle(ToggleCategoryAttributeCommand command)
    {
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        // Find the category attribute relationship
        var categoryAttribute = await _context.CategoryAttributes
            .FirstOrDefaultAsync(ca => ca.CategoryId == command.CategoryId && ca.DynamicAttributeId == command.DynamicAttributeId);

        if (categoryAttribute is null)
        {
            return await _createCategoryAttributeCommandHandler.Handle(new CreateCategoryAttributeCommand
            {
                CategoryId = command.CategoryId,
                DynamicAttributeId = command.DynamicAttributeId,
                UserId = command.UserId,
                Language = command.Language
            });
        }
        else
        {
            return await _deleteCategoryAttributeCommandHandler.Handle(new DeleteCategoryAttributeCommand
            {
                CategoryId = command.CategoryId,
                DynamicAttributeId = command.DynamicAttributeId,
                UserId = command.UserId,
                Language = command.Language
            });
        }
    }
}