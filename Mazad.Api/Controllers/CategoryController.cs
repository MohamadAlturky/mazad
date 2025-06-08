using Mazad.Core.Domain.Users.Authentication;
using Mazad.UseCases.Categories.Create;
using Mazad.UseCases.Categories.Delete;
using Mazad.UseCases.Categories.Read;
using Mazad.UseCases.Categories.Update;
using Mazad.UseCases.Categories.Toggle;
using Microsoft.AspNetCore.Mvc;
using Mazad.UseCases.CategoryDomain.CategoryAttributes.Read;
using Mazad.UseCases.CategoryDomain.CategoryAttributes.Toggle;

namespace Mazad.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : BaseController
{
    private readonly CreateCategoryCommandHandler _createCategoryCommandHandler;
    private readonly UpdateCategoryCommandHandler _updateCategoryCommandHandler;
    private readonly DeleteCategoryCommandHandler _deleteCategoryCommandHandler;
    private readonly GetAllCategoriesQueryHandler _getAllCategoriesQueryHandler;
    private readonly GetCategoriesDropdownQueryHandler _getCategoriesDropdownQueryHandler;
    private readonly GetCategoriesListQueryHandler _getCategoriesListQueryHandler;
    private readonly ToggleCategoryActivationCommandHandler _toggleCategoryActivationCommandHandler;
    private readonly GetCategoriesTreeByOneQueryHandler _getCategoriesTreeByOneQueryHandler;
    private readonly GetCategoryBasicInfoQueryHandler _getCategoryBasicInfoQueryHandler;
    private readonly GetDynamicAttributesByCategoryIdQueryHandler _getDynamicAttributesByCategoryIdQueryHandler;
    private readonly ToggleCategoryAttributeCommandHandler _toggleCategoryAttributeCommandHandler;
    public CategoryController(
        CreateCategoryCommandHandler createCategoryCommandHandler,
        UpdateCategoryCommandHandler updateCategoryCommandHandler,
        DeleteCategoryCommandHandler deleteCategoryCommandHandler,
        GetAllCategoriesQueryHandler getAllCategoriesQueryHandler,
        GetCategoriesDropdownQueryHandler getCategoriesDropdownQueryHandler,
        GetCategoriesListQueryHandler getCategoriesListQueryHandler,
        GetCategoriesTreeByOneQueryHandler getCategoriesTreeByOneQueryHandler,
        ToggleCategoryActivationCommandHandler toggleCategoryActivationCommandHandler,
        GetCategoryBasicInfoQueryHandler getCategoryBasicInfoQueryHandler,
        GetDynamicAttributesByCategoryIdQueryHandler getDynamicAttributesByCategoryIdQueryHandler,
        ToggleCategoryAttributeCommandHandler toggleCategoryAttributeCommandHandler
    )
    {
        _createCategoryCommandHandler = createCategoryCommandHandler;
        _updateCategoryCommandHandler = updateCategoryCommandHandler;
        _deleteCategoryCommandHandler = deleteCategoryCommandHandler;
        _getAllCategoriesQueryHandler = getAllCategoriesQueryHandler;
        _getCategoriesDropdownQueryHandler = getCategoriesDropdownQueryHandler;
        _getCategoriesListQueryHandler = getCategoriesListQueryHandler;
        _getCategoriesTreeByOneQueryHandler = getCategoriesTreeByOneQueryHandler;
        _toggleCategoryActivationCommandHandler = toggleCategoryActivationCommandHandler;
        _getCategoryBasicInfoQueryHandler = getCategoryBasicInfoQueryHandler;
        _getDynamicAttributesByCategoryIdQueryHandler = getDynamicAttributesByCategoryIdQueryHandler;
        _toggleCategoryAttributeCommandHandler = toggleCategoryAttributeCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _getAllCategoriesQueryHandler.Handle(new GetAllCategoriesQuery()
        {
            Language = GetLanguage(),
            UserId = GetUserId()
        });
        return Represent(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryApiRequest request)
    {
        var command = request.ToCommand(GetUserId(), GetLanguage());
        var result = await _createCategoryCommandHandler.Handle(command);
        return Represent(result);
    }
    [HttpPut]
    public async Task<IActionResult> Update(UpdateCategoryApiRequest request)
    {
        var command = request.ToCommand(GetUserId(), GetLanguage());
        var result = await _updateCategoryCommandHandler.Handle(command);
        return Represent(result);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCategoryApiRequest request)
    {
        var command = request.ToCommand(GetUserId(), GetLanguage());
        var result = await _deleteCategoryCommandHandler.Handle(command);
        return Represent(result);
    }

    [HttpGet("dropdown")]
    public async Task<IActionResult> GetCategoriesDropdown()
    {
        var result = await _getCategoriesDropdownQueryHandler.Handle(new GetCategoriesDropdownQuery()
        {
            Language = GetLanguage(),
            UserId = GetUserId()
        });
        return Represent(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetCategoriesList(
        [FromQuery] bool? filterByIsActiveEquals,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _getCategoriesListQueryHandler.Handle(new GetCategoriesListQuery
        {
            FilterByIsActiveEquals = filterByIsActiveEquals,
            PageSize = pageSize,
            PageNumber = pageNumber,
            Language = GetLanguage(),
            UserId = GetUserId()

        });
        return Represent(result);
    }

    [HttpPut("toggle-activation/{id}")]
    public async Task<IActionResult> ToggleActivation(int id)
    {
        var command = new ToggleCategoryActivationCommand
        {
            CategoryId = id,
            Language = GetLanguage(),
            UserId = GetUserId()
        };
        var result = await _toggleCategoryActivationCommandHandler.Handle(command);
        return Represent(result);
    }

    [HttpGet("tree/{id}")]
    public async Task<IActionResult> GetCategoriesTree(int id)
    {
        var result = await _getCategoriesTreeByOneQueryHandler.Handle(new GetCategoriesTreeByOneQuery
        {
            CategoryId = id,
            Language = GetLanguage(),
            UserId = GetUserId()
        });
        return Represent(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBasicInfo(int id)
    {
        var result = await _getCategoryBasicInfoQueryHandler.Handle(new GetCategoryBasicInfoQuery
        { 
            Id = id,
            Language = GetLanguage(),
            UserId = GetUserId()
        });
        return Represent(result);
    }

    [HttpGet("attributes/{id}")]
    public async Task<IActionResult> GetDynamicAttributesByCategoryId(int id)
    {
        var result = await _getDynamicAttributesByCategoryIdQueryHandler.Handle(new GetDynamicAttributesByCategoryIdQuery
        {
            CategoryId = id,
            Language = GetLanguage(),
            UserId = GetUserId()
        });
        return Represent(result);
    }

    [HttpPut("attributes/{categoryId}/{dynamicAttributeId}")]
    public async Task<IActionResult> ToggleCategoryAttribute(int categoryId, int dynamicAttributeId)
    {
        var command = new ToggleCategoryAttributeCommand
        {
            CategoryId = categoryId,
            DynamicAttributeId = dynamicAttributeId,
            Language = GetLanguage(),
            UserId = GetUserId()
        };
        var result = await _toggleCategoryAttributeCommandHandler.Handle(command);
        return Represent(result);
    }
}