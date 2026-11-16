using Microsoft.AspNetCore.Identity;

namespace Psikotes.Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
