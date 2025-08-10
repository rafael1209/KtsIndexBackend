using KtsIndexBackend.Enums;

namespace KtsIndexBackend.Services;

public class SetAccessRequest
{
    public required string UserId { get; set; } = string.Empty;
    public PermissionLevel Permission { get; set; }
}