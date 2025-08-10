using KtsIndexBackend.Enums;
using System.Security.Claims;

namespace KtsIndexBackend.Interfaces;

public interface ITokenService
{
    string GenerateToken(string value, PermissionLevel level = PermissionLevel.User);
    ClaimsPrincipal? ValidateToken(string token);
    PermissionLevel? GetUserPermissionFromToken(string token);
}