using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Middlewares;
using KtsIndexBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KtsIndexBackend.Controllers;

[Route("api/v1/users")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet("validate")]
    [AuthMiddleware]
    public async Task<IActionResult> Validate([FromBody] JsonElement body)
    {
        try
        {
            var authToken = await userService.AuthorizeUser(body);

            return Ok(new { authToken });
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("me")]
    [AuthMiddleware]
    public async Task<IActionResult> GetMe()
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        try
        {
            var userInfo = await userService.GetUserInfo(jwtData.Id);

            return Ok(userInfo);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}