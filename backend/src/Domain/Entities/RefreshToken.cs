using Ardalis.GuardClauses;

namespace Psikotes.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;

    public void Revoke(DateTime revokedAtUtc)
    {
        Guard.Against.Default(revokedAtUtc, nameof(revokedAtUtc));

        if (RevokedAtUtc is not null)
        {
            throw new InvalidOperationException("Token sudah dicabut sebelumnya.");
        }

        RevokedAtUtc = revokedAtUtc;
    }

    public void SetReplacedByToken(string replacedByToken)
    {
        Guard.Against.NullOrWhiteSpace(replacedByToken, nameof(replacedByToken));

        ReplacedByToken = replacedByToken;
    }
}
