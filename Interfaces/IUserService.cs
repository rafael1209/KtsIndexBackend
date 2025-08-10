using KtsIndexBackend.Models;
using KtsIndexBackend.Services;
using MongoDB.Bson;

namespace KtsIndexBackend.Interfaces;

public interface IUserService
{
    Task<User?> TryGetByMinecraftUuid(string minecraftUuid);
    Task<User> Create(User user);
    Task<User?> GetById(ObjectId id);
    Task SetAccess(JwtData userData, SetAccessRequest request);
}