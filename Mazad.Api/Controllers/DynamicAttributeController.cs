using Mazad.Core.Domain.Users.Authentication;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Create;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Delete;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Read;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Toggle;
using Mazad.UseCases.CategoryDomain.DynamicAttributes.Update;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Api.Controllers;

[ApiController]
[Route("api/dynamic-attributes")]
public class DynamicAttributeController : BaseController
{
    private readonly CreateDynamicAttributeCommandHandler _createDynamicAttributeCommandHandler;
    private readonly UpdateDynamicAttributeCommandHandler _updateDynamicAttributeCommandHandler;
    private readonly DeleteDynamicAttributeCommandHandler _deleteDynamicAttributeCommandHandler;
    private readonly GetAllDynamicAttributesQueryHandler _getAllDynamicAttributesQueryHandler;
    private readonly ToggleDynamicAttributeCommandHandler _toggleDynamicAttributeCommandHandler;
    private readonly GetDynamicAttributesBasicInfoQueryHandler _getDynamicAttributesBasicInfoQueryHandler;
    public DynamicAttributeController(
        CreateDynamicAttributeCommandHandler createDynamicAttributeCommandHandler,
        UpdateDynamicAttributeCommandHandler updateDynamicAttributeCommandHandler,
        DeleteDynamicAttributeCommandHandler deleteDynamicAttributeCommandHandler,
        GetAllDynamicAttributesQueryHandler getAllDynamicAttributesQueryHandler,
        ToggleDynamicAttributeCommandHandler toggleDynamicAttributeCommandHandler,
        GetDynamicAttributesBasicInfoQueryHandler getDynamicAttributesBasicInfoQueryHandler
    )
    {
        _createDynamicAttributeCommandHandler = createDynamicAttributeCommandHandler;
        _updateDynamicAttributeCommandHandler = updateDynamicAttributeCommandHandler;
        _deleteDynamicAttributeCommandHandler = deleteDynamicAttributeCommandHandler;
        _getAllDynamicAttributesQueryHandler = getAllDynamicAttributesQueryHandler;
        _toggleDynamicAttributeCommandHandler = toggleDynamicAttributeCommandHandler;
        _getDynamicAttributesBasicInfoQueryHandler = getDynamicAttributesBasicInfoQueryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _getAllDynamicAttributesQueryHandler.Handle(new GetAllDynamicAttributesQuery()
        {
            Language = GetLanguage(),
            UserId = GetUserId()
        });
        return Represent(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDynamicAttributeApiRequest request)
    {
        var command = request.ToCommand(GetUserId(), GetLanguage());
        var result = await _createDynamicAttributeCommandHandler.Handle(command);
        return Represent(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateDynamicAttributeApiRequest request)
    {
        var command = request.ToCommand(GetUserId(), GetLanguage());
        var result = await _updateDynamicAttributeCommandHandler.Handle(command);
        return Represent(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDynamicAttributeApiRequest request)
    {
        var command = request.ToCommand(GetUserId(), GetLanguage());
        var result = await _deleteDynamicAttributeCommandHandler.Handle(command);
        return Represent(result);
    }

    [HttpPut("toggle-activation/{id}")]
    public async Task<IActionResult> ToggleActivation(int id)
    {
        var command = new ToggleDynamicAttributeCommand
        {
            Id = id,
            Language = GetLanguage(),
            UserId = GetUserId()
        };
        var result = await _toggleDynamicAttributeCommandHandler.Handle(command);
        return Represent(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBasicInfo(int id)
    {
        var result = await _getDynamicAttributesBasicInfoQueryHandler.Handle(new GetDynamicAttributesBasicInfoQuery { Id = id, Language = GetLanguage(), UserId = GetUserId() });
        return Represent(result);
    }
}