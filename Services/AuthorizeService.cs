﻿using KtsIndexBackend.Helpers;
using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Models;
using MongoDB.Bson;
using System.Text.Json;

namespace KtsIndexBackend.Services;

public class AuthorizeService(IConfiguration configuration, IUserService userService, ITokenService tokenService) : IAuthorizeService
{
    private readonly string _miniAppToken = configuration["SPWorlds:MiniAppToken"] ??
                                            throw new Exception("MiniAppToken configuration is missing.");

    public async Task<string> AuthorizeUser(JsonElement body)
    {
        var properties = new Dictionary<string, object?>();
        foreach (var prop in body.EnumerateObject())
        {
            var value = prop.Value;

            properties[prop.Name] = value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.False => "false",
                JsonValueKind.True => "true",
                JsonValueKind.Array when value.GetArrayLength() == 0 => "",
                _ => value.ToString()
            };
        }

        var isValidate = SpValidate.CheckUser(properties, _miniAppToken);

        if (!isValidate)
            throw new Exception("User validation failed.");

        if (!properties.TryGetValue("minecraftUUID", out var uuidObj) || uuidObj is not string minecraftUuid ||
            string.IsNullOrWhiteSpace(minecraftUuid))
            throw new Exception("Missing or invalid minecraftUUID");

        if (!properties.TryGetValue("username", out var usernameObj) || usernameObj is not string username ||
            string.IsNullOrWhiteSpace(username))
            throw new Exception("Missing or invalid username");

        var userId = ObjectId.GenerateNewId();

        var user = await userService.TryGetByMinecraftUuid(minecraftUuid)
                   ?? await userService.Create(new User
                   {
                       Id = userId,
                       AuthToken = tokenService.GenerateToken(userId.ToString()),
                       MinecraftUuid = minecraftUuid,
                       Username = username
                   });

        return user.AuthToken;
    }
}