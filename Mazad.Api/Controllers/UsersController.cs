using Mazad.Core.Domain.Users.Authentication;
using Mazad.UseCases.Users.Login;
using Mazad.UseCases.Users.Profile;
using Mazad.UseCases.Users.Read;
using Mazad.UseCases.Users.Register;
using Mazad.UseCases.Users.VerifyOtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : BaseController
{
    private readonly RegisterUserCommandHandler _registerUserCommandHandler;
    private readonly VerifyOtpCommandHandler _verifyOtpCommandHandler;
    private readonly LoginCommandHandler _loginCommandHandler;
    private readonly GetUserProfileQueryHandler _getUserProfileQueryHandler;
    private readonly GetUsersListQueryHandler _getUsersListQueryHandler;

    public UsersController(
        RegisterUserCommandHandler registerUserCommandHandler,
        VerifyOtpCommandHandler verifyOtpCommandHandler,
        LoginCommandHandler loginCommandHandler,
        GetUserProfileQueryHandler getUserProfileQueryHandler,
        GetUsersListQueryHandler getUsersListQueryHandler
    )
    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _verifyOtpCommandHandler = verifyOtpCommandHandler;
        _loginCommandHandler = loginCommandHandler;
        _getUserProfileQueryHandler = getUserProfileQueryHandler;
        _getUsersListQueryHandler = getUsersListQueryHandler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginApiRequest request)
    {
        var result = await _loginCommandHandler.Handle(
            request.ToCommand(GetUserId(), GetLanguage())
        );
        return Represent(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserApiRequest request)
    {
        var result = await _registerUserCommandHandler.Handle(
            request.ToCommand(GetUserId(), GetLanguage())
        );
        return Represent(result);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpApiRequest request)
    {
        var result = await _verifyOtpCommandHandler.Handle(
            request.ToCommand(GetUserId(), GetLanguage())
        );
        return Represent(result);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var result = await _getUserProfileQueryHandler.Handle(
            new GetUserProfileQuery { UserId = GetUserId(), Language = GetLanguage() }
        );
        return Represent(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetUsersList(
        [FromQuery] bool? filterByIsActiveEquals,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageNumber = 1,
        [FromQuery] string? searchTerm = null
    )
    {
        var request = new GetUsersListQuery
        {
            FilterByIsActiveEquals = filterByIsActiveEquals,
            PageSize = pageSize,
            PageNumber = pageNumber,
            SearchTerm = searchTerm,
            Language = GetLanguage(),
            UserId = GetUserId(),
        };
        var result = await _getUsersListQueryHandler.Handle(request);
        return Represent(result);
    }
}
