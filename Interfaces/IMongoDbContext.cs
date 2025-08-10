using KtsIndexBackend.Models;
using MongoDB.Driver;

namespace KtsIndexBackend.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<User> UsersCollection { get; }
}