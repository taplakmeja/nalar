namespace Psikotes.Application.UseCases.Auth;

public sealed record UserDto(string Id, string Email, string DisplayName, IReadOnlyList<string> Roles);
