using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Models;

namespace KtsIndexBackend.Services;

public class UserService : IUserService
{
    public Task<User?> TryGetByMinecraftUuid(string minecraftUuid)
    {
        throw new NotImplementedException();
    }

    public Task<User> Create(User user)
    {
        throw new NotImplementedException();
    }
}