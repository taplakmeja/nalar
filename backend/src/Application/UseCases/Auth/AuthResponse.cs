namespace Psikotes.Application.UseCases.Auth;

public sealed record AuthResponse(string AccessToken, string RefreshToken, UserDto User);
