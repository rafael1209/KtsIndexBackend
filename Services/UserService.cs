using System.Text.Json;
using KtsIndexBackend.Enums;
using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Models;
using MongoDB.Bson;

namespace KtsIndexBackend.Services;

public class UserService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IAuthorizeService authorizeService)
    : IUserService
{
    public async Task<User?> TryGetByMinecraftUuid(string minecraftUuid)
    {
        return await userRepository.TryGetByMinecraftUuid(minecraftUuid);
    }

    public async Task<User> Create(User user)
    {
        return await userRepository.Create(user);
    }

    public async Task<User?> GetById(ObjectId id)
    {
        return await userRepository.GetById(id);
    }

    public async Task<UserInfo> GetUserInfo(string userId)
    {
        if (!ObjectId.TryParse(userId, out var objectId))
            throw new ArgumentException("Invalid ObjectId format", nameof(userId));

        var user = await userRepository.GetById(objectId) ?? throw new Exception("User not found.");
        var permission = tokenService.GetUserPermissionFromToken(user.AuthToken);

        var userInfo = new UserInfo
        {
            Id = user.Id.ToString(),
            Nickname = user.Username,
            Permission = permission
        };

        return userInfo;
    }

    public async Task<string> AuthorizeUser(JsonElement body)
    {
        return await authorizeService.AuthorizeUser(body);
    }

    public async Task SetAccess(JwtData userData, SetAccessRequest request)
    {
        if (!ObjectId.TryParse(request.UserId, out var userIdObj))
            throw new ArgumentException("Invalid ObjectId format", nameof(request.UserId));

        var secondUser = await GetById(userIdObj) ??
                         throw new Exception("User not found.");

        var permissionLevel = tokenService.GetUserPermissionFromToken(secondUser.AuthToken);

        if (permissionLevel >= userData.Permission && userData.Permission != PermissionLevel.Owner)
            throw new UnauthorizedAccessException("You cannot set a higher permission level than your own.");

        var authToken = tokenService.GenerateToken(request.UserId, request.Permission);

        await userRepository.UpdateAuthToken(userIdObj, authToken);
    }
}