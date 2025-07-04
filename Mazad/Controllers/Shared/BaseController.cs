using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Api.Controllers;

public class BaseController : ControllerBase
{
    public BaseController() { }

    protected string GetLanguage()
    {
        return Request.Headers["Accept-Language"].ToString() ?? "en";
    }

    protected int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    protected IActionResult Represent(
        bool success,
        LocalizedMessage message,
        Exception? exception = null
    )
    {
        if (success)
        {
            return Ok(
                new ApiResponse
                {
                    Success = success,
                    Message = GetLanguage() == "ar" ? message.Arabic : message.English,
                }
            );
        }

        return Ok(
            new ApiResponse
            {
                Success = success,
                Message = GetLanguage() == "ar" ? message.Arabic : message.English,
                Exception = exception?.Message ?? string.Empty,
                StackTrace = exception?.StackTrace ?? string.Empty,
            }
        );
    }

    protected IActionResult Represent<T>(
        T data,
        bool success,
        LocalizedMessage message,
        Exception? exception = null
    )
    {
        if (success)
        {
            return Ok(
                new ApiResponse<T>
                {
                    Success = success,
                    Message = GetLanguage() == "ar" ? message.Arabic : message.English,
                    Data = data,
                }
            );
        }

        return Ok(
            new ApiResponse<T>
            {
                Success = success,
                Message = GetLanguage() == "ar" ? message.Arabic : message.English,
                Data = data,
                Exception = exception?.Message ?? string.Empty,
                StackTrace = exception?.StackTrace ?? string.Empty,
            }
        );
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
}

public class LocalizedMessage
{
    public string Arabic { get; set; } = string.Empty;
    public string English { get; set; } = string.Empty;
}
