using Microsoft.AspNetCore.Identity;

namespace Aldaman.Persistence.Entities;

public class AppUser : IdentityUser<Guid>
{
    public const int DisplayNameMaxLength = 256;

    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
