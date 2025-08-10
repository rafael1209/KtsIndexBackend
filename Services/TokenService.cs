﻿using System.IdentityModel.Tokens.Jwt;
using KtsIndexBackend.Enums;
using System.Security.Claims;
using System.Text;
using KtsIndexBackend.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace KtsIndexBackend.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _secretKey =
        configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt Key configuration is missing.");
    private readonly string _issuer =
        configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt Issuer configuration is missing.");

    public string GenerateToken(string userId, PermissionLevel permission = PermissionLevel.User)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("permission", permission.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = null
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public PermissionLevel? GetUserPermissionFromToken(string token)
    {
        var principal = ValidateToken(token);

        var permissionClaim = principal?.FindFirst("permission");

        if (permissionClaim != null && Enum.TryParse(permissionClaim.Value, out PermissionLevel permissionLevel))
            return permissionLevel;

        return null;
    }
}