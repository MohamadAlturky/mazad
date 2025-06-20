using Mazad.Core.Domain.Users.Authentication;
using Mazad.UseCases.Users.Login;
using Mazad.UseCases.Users.Profile;
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

    public UsersController(
        RegisterUserCommandHandler registerUserCommandHandler,
        VerifyOtpCommandHandler verifyOtpCommandHandler,
        LoginCommandHandler loginCommandHandler,
        GetUserProfileQueryHandler getUserProfileQueryHandler
    )
    {
        _registerUserCommandHandler = registerUserCommandHandler;
        _verifyOtpCommandHandler = verifyOtpCommandHandler;
        _loginCommandHandler = loginCommandHandler;
        _getUserProfileQueryHandler = getUserProfileQueryHandler;
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
}
