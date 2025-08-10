using KtsIndexBackend.Models;
using KtsIndexBackend.Services;
using MongoDB.Bson;
using System.Text.Json;

namespace KtsIndexBackend.Interfaces;

public interface IUserService
{
    Task<User?> TryGetByMinecraftUuid(string minecraftUuid);
    Task<User> Create(User user);
    Task<User?> GetById(ObjectId id);
    Task SetAccess(JwtData userData, SetAccessRequest request);
    Task<UserInfo> GetUserInfo(string userId);
    Task<string> AuthorizeUser(JsonElement body);
}