namespace Psikotes.Application.UseCases.Auth;

public sealed record AuthResponse(string AccessToken, string RefreshToken, UserDto User);

public sealed record UserDto(string Id, string Email, string DisplayName, IReadOnlyList<string> Roles);
