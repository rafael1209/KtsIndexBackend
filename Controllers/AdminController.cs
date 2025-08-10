using KtsIndexBackend.Enums;
using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Middlewares;
using KtsIndexBackend.Models;
using KtsIndexBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace KtsIndexBackend.Controllers;

[Route("api/v1/admin")]
public class AdminController(IUserService userService, ITokenService tokenService) : Controller
{
    [HttpPost("set-access")]
    [AuthMiddleware(PermissionLevel.Admin)]
    public async Task<IActionResult> SetAccess([FromBody] SetAccessRequest request)
    {
        try
        {
            var jwtData = HttpContext.Items["@me"] as JwtData
                          ?? throw new UnauthorizedAccessException();

            await userService.SetAccess(jwtData, request);

            return Ok(new { message = "Access level updated successfully." });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("generate-owner")]
    [AuthMiddleware(PermissionLevel.Owner)]
    public Task<IActionResult> Test([FromBody] string userId)
    {
        try
        {
            var authToken = tokenService.GenerateToken(userId, PermissionLevel.Owner);

            return Task.FromResult<IActionResult>(Ok(new { authToken }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return Task.FromResult<IActionResult>(BadRequest(new { message = e.Message }));
        }
    }
}