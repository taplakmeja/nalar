namespace Psikotes.Domain.Common;

public static class Roles
{
    public const string User = "User";
    public const string Administrator = "Administrator";
    public const string ContentManager = "ContentManager";

    public static readonly IReadOnlyList<string> All = [User, ContentManager, Administrator];
}
