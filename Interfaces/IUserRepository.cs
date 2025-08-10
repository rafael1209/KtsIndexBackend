﻿using KtsIndexBackend.Models;
using MongoDB.Bson;

namespace KtsIndexBackend.Interfaces;

public interface IUserRepository
{
    Task<User?> TryGetByMinecraftUuid(string minecraftUuid);
    Task<User?> TryGetByAuthToken(string authToken);
    Task<User> Create(User user);
    Task<User?> GetById(ObjectId id);
    Task UpdateAuthToken(ObjectId userId, string authToken);
}