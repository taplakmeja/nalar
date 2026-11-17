using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Psikotes.Application.UseCases.Auth;
using Psikotes.Application.UseCases.Auth.Commands.Login;
using Psikotes.Application.UseCases.Auth.Commands.Register;
using Psikotes.Domain.Common;
using Psikotes.Domain.Entities;
using Xunit;

namespace Psikotes.Web.IntegrationTests;

[Collection("Integration")]
public class RoleGuardTests
{
    private readonly PsikotesWebApplicationFactory _factory;

    public RoleGuardTests(PsikotesWebApplicationFactory factory) => _factory = factory;

    private async Task<string> RegisterAndGetTokenAsync(string email, string? role = null)
    {
        var client = _factory.CreateClient();
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand(
            Email: email,
            Password: "Password#123",
            DisplayName: "Pengguna Uji"));
        registerResponse.EnsureSuccessStatusCode();

        if (role is not null)
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(email);
            Assert.NotNull(user);
            var result = await userManager.AddToRoleAsync(user!, role);
            Assert.True(result.Succeeded);

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand(email, "Password#123"));
            loginResponse.EnsureSuccessStatusCode();
            var login = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            return login!.AccessToken;
        }

        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.AccessToken;
    }

    [Fact]
    public async Task AdminEndpoints_AsPlainUser_Return403()
    {
        var email = $"plain-{Guid.NewGuid():N}@test.local";
        var token = await RegisterAndGetTokenAsync(email);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var users = await client.GetAsync("/api/admin/users");
        var settings = await client.GetAsync("/api/admin/settings");

        Assert.Equal(HttpStatusCode.Forbidden, users.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, settings.StatusCode);
    }

    [Fact]
    public async Task GeneratorEndpoints_AsPlainUser_Return403()
    {
        var email = $"plain2-{Guid.NewGuid():N}@test.local";
        var token = await RegisterAndGetTokenAsync(email);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var list = await client.GetAsync("/api/generator-configs");
        var create = await client.PostAsJsonAsync("/api/generator-configs", new { name = "x", category = "Numerical", configJson = "{}" });

        Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, create.StatusCode);
    }

    [Fact]
    public async Task AdminUsers_AsAdministrator_Return200()
    {
        var email = $"admin-{Guid.NewGuid():N}@test.local";
        var token = await RegisterAndGetTokenAsync(email, Roles.Administrator);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/admin/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GeneratorEndpoints_AsContentManager_Return200()
    {
        var email = $"manager-{Guid.NewGuid():N}@test.local";
        var token = await RegisterAndGetTokenAsync(email, Roles.ContentManager);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/generator-configs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
