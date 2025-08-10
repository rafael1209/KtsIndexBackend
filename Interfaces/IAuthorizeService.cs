using System.Text.Json;

namespace KtsIndexBackend.Interfaces;

public interface IAuthorizeService
{
    Task<string> AuthorizeUser(JsonElement body);
}