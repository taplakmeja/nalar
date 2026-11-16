using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Psikotes.Application.UseCases.Auth;
using Psikotes.Application.UseCases.Auth.Commands.Login;
using Psikotes.Application.UseCases.Auth.Commands.Logout;
using Psikotes.Application.UseCases.Auth.Commands.Refresh;
using Psikotes.Application.UseCases.Auth.Commands.Register;
using Xunit;

namespace Psikotes.Web.IntegrationTests;

[Collection("Integration")]
public class AuthFlowTests
{
    private readonly PsikotesWebApplicationFactory _factory;

    public AuthFlowTests(PsikotesWebApplicationFactory factory) => _factory = factory;

    private static RegisterCommand NewRegisterCommand() => new(
        Email: $"user-{Guid.NewGuid():N}@test.local",
        Password: "Password#123",
        DisplayName: "Pengguna Uji");

    [Fact]
    public async Task Register_ThenLogin_ReturnsTokens()
    {
        var client = _factory.CreateClient();
        var request = NewRegisterCommand();

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(registered!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(registered.RefreshToken));
        Assert.Contains("User", registered.User.Roles);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand(request.Email, request.Password));
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var login = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(login!.AccessToken));
    }

    [Fact]
    public async Task Me_WithValidToken_ReturnsUser()
    {
        var client = _factory.CreateClient();
        var request = NewRegisterCommand();
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        var me = await client.GetFromJsonAsync<UserDto>("/api/me");

        Assert.Equal(request.Email, me!.Email);
        Assert.Contains("User", me.Roles);
    }

    [Fact]
    public async Task Me_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminPing_AsRegularUser_Returns403()
    {
        var client = _factory.CreateClient();
        var request = NewRegisterCommand();
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        var response = await client.GetAsync("/api/admin/ping");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndOldTokenIsRejected()
    {
        var client = _factory.CreateClient();
        var request = NewRegisterCommand();
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshCommand(auth!.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshed = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotEqual(auth.RefreshToken, refreshed!.RefreshToken);

        var reuseResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshCommand(auth.RefreshToken));
        Assert.Equal(HttpStatusCode.BadRequest, reuseResponse.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        var client = _factory.CreateClient();
        var request = NewRegisterCommand();

        await client.PostAsJsonAsync("/api/auth/register", request);
        var second = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task HealthEndpoints_Return200()
    {
        var client = _factory.CreateClient();

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/live")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready")).StatusCode);
    }
}
