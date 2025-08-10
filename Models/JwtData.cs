using KtsIndexBackend.Enums;

namespace KtsIndexBackend.Models;

public class JwtData
{
    public required string Id { get; set; }
    public PermissionLevel Permission { get; set; }
}