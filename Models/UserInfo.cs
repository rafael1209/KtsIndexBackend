using KtsIndexBackend.Enums;
using System.Text.Json.Serialization;

namespace KtsIndexBackend.Models;

public class UserInfo
{
    public required string Id { get; set; }
    public required string Nickname { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required PermissionLevel? Permission { get; set; }
}