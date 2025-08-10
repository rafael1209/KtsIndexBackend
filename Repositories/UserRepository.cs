using KtsIndexBackend.Interfaces;
using KtsIndexBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KtsIndexBackend.Repositories;

public class UserRepository(IMongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _users = context.UsersCollection;

    public async Task<User?> TryGetByMinecraftUuid(string minecraftUuid)
    {
        var filter = Builders<User>.Filter.Eq(u => u.MinecraftUuid, minecraftUuid);

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> TryGetByAuthToken(string authToken)
    {
        var filter = Builders<User>.Filter.Eq(u => u.AuthToken, authToken);

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User> Create(User user)
    {
        await _users.InsertOneAsync(user);

        return user;
    }

    public async Task<User?> GetById(ObjectId id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAuthToken(ObjectId userId, string authToken)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.Set(u => u.AuthToken, authToken);

        var result = await _users.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0)
            throw new Exception("Failed to update auth token for user.");
    }
}