using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mazad.Core.Domain.Users.Authentication;

namespace Mazad.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // In a real application, you would validate user credentials here
        // e.g., check username and password against a database
        if (request.Username == "testuser" && request.Password == "password")
        {
            // Assuming a successful login, generate a token for user ID 1
            var userId = 1;
            var token = _jwtService.GenerateToken(userId, request.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized("Invalid credentials.");
    }

    [HttpGet("protected-resource")]
    [Authorize] // This attribute protects the endpoint, requiring a valid JWT
    public IActionResult GetProtectedResource()
    {
        // Get the user ID from the JWT claims
        var userId = User.GetUserId(); // Using the extension method
        var userName = User.Identity?.Name; // Get the username claim

        if (userId.HasValue)
        {
            return Ok($"Hello, user {userName}! Your ID is: {userId.Value}. You accessed a protected resource.");
        }

        return Unauthorized("User ID not found in token.");
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}