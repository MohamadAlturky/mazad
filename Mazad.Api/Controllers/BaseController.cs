using System.Security.Claims;
using Mazad.Core.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Mazad.Api.Controllers;

public class BaseController : ControllerBase
{

    public BaseController()
    {
    }

    protected string GetLanguage()
    {
        return Request.Headers["Accept-Language"].ToString() ?? "en";
    }

    protected int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    protected IActionResult Represent(Result result)
    {
        if (result.Success)
        {
            return Ok(new ApiResponse
            {
                Success = result.Success,
                Message = GetLanguage() == "ar" ? result.Message.Arabic : result.Message.English,
            });
        }

        return Ok(new ApiResponse
        {
            Success = result.Success,
            Message = GetLanguage() == "ar" ? result.Message.Arabic : result.Message.English,
            Exception = result.Exception?.Message ?? string.Empty,
            StackTrace = result.Exception?.StackTrace ?? string.Empty,
        });
    }

    protected IActionResult Represent<T>(Result<T> result)
    {
        if (result.Success)
        {
            return Ok(new ApiResponse<T>
            {
                Success = result.Success,
                Message = GetLanguage() == "ar" ? result.Message.Arabic : result.Message.English,
                Data = result.Data,
            });
        }

        return Ok(new ApiResponse
        {
            Success = result.Success,
            Message = GetLanguage() == "ar" ? result.Message.Arabic : result.Message.English,
            Exception = result.Exception?.Message ?? string.Empty,
            StackTrace = result.Exception?.StackTrace ?? string.Empty,
        });
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
