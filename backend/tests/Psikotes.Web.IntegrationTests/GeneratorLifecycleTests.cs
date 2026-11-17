using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Psikotes.Application.UseCases.Auth;
using Psikotes.Application.UseCases.Auth.Commands.Login;
using Psikotes.Application.UseCases.Auth.Commands.Register;
using Psikotes.Application.UseCases.Generators.Dtos;
using Psikotes.Domain.Common;
using Psikotes.Domain.Entities;
using Xunit;

namespace Psikotes.Web.IntegrationTests;

[Collection("Integration")]
public class GeneratorLifecycleTests
{
    private const string ConfigJson = """
        {
          "promptTemplate": "Berapakah hasil dari {a} + {b}?",
          "parameters": [
            { "name": "a", "min": 1, "max": 20 },
            { "name": "b", "min": 1, "max": 20 }
          ],
          "operations": [ { "name": "total", "expression": "a + b" } ],
          "answerExpression": "total",
          "distractorExpressions": ["total + 1", "total - 1", "total + 2"],
          "optionsCount": 4
        }
        """;

    private readonly PsikotesWebApplicationFactory _factory;

    public GeneratorLifecycleTests(PsikotesWebApplicationFactory factory) => _factory = factory;

    private async Task<HttpClient> CreateContentManagerClientAsync()
    {
        var email = $"manager-{Guid.NewGuid():N}@test.local";
        var client = _factory.CreateClient();
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand(
            Email: email,
            Password: "Password#123",
            DisplayName: "Manajer Konten"));
        registerResponse.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        var roleResult = await userManager.AddToRoleAsync(user!, Roles.ContentManager);
        Assert.True(roleResult.Succeeded);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand(email, "Password#123"));
        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);
        return client;
    }

    [Fact]
    public async Task Create_MassTest_Publish_Rollback_FullLifecycle_Succeeds()
    {
        var client = await CreateContentManagerClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/generator-configs", new
        {
            name = "Penjumlahan Uji " + Guid.NewGuid().ToString("N"),
            category = "Numerical",
            configJson = ConfigJson
        });
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<GeneratorConfigDto>();
        Assert.Equal("Draft", created!.Status);
        Assert.Equal(1, created.Version);

        var publishWithoutTest = await client.PostAsync($"/api/generator-configs/{created.Id}/publish", null);
        Assert.Equal(HttpStatusCode.BadRequest, publishWithoutTest.StatusCode);

        var massTestResponse = await client.PostAsync($"/api/generator-configs/{created.Id}/mass-test", null);
        Assert.Equal(HttpStatusCode.OK, massTestResponse.StatusCode);
        var report = await massTestResponse.Content.ReadFromJsonAsync<MassTestReportDto>();
        Assert.True(report!.Passed);

        var publishResponse = await client.PostAsync($"/api/generator-configs/{created.Id}/publish", null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        var published = await publishResponse.Content.ReadFromJsonAsync<GeneratorConfigDto>();
        Assert.Equal("Published", published!.Status);

        var versionResponse = await client.PostAsJsonAsync($"/api/generator-configs/{created.Id}/versions", new { configJson = ConfigJson });
        Assert.Equal(HttpStatusCode.OK, versionResponse.StatusCode);
        var draftV2 = await versionResponse.Content.ReadFromJsonAsync<GeneratorConfigDto>();
        Assert.Equal(2, draftV2!.Version);

        await client.PostAsync($"/api/generator-configs/{draftV2.Id}/mass-test", null);
        var publishV2Response = await client.PostAsync($"/api/generator-configs/{draftV2.Id}/publish", null);
        Assert.Equal(HttpStatusCode.OK, publishV2Response.StatusCode);

        var rollbackResponse = await client.PostAsync($"/api/generator-configs/rollback/{created.GeneratorId}", null);
        Assert.Equal(HttpStatusCode.OK, rollbackResponse.StatusCode);
        var rolledBack = await rollbackResponse.Content.ReadFromJsonAsync<GeneratorConfigDto>();
        Assert.Equal(1, rolledBack!.Version);
        Assert.Equal("Published", rolledBack.Status);
    }

    [Fact]
    public async Task Preview_WithValidConfig_ReturnsQuestions()
    {
        var client = await CreateContentManagerClientAsync();

        var response = await client.PostAsJsonAsync("/api/generator-configs/preview", new
        {
            category = "Numerical",
            configJson = ConfigJson,
            samples = 2
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var questions = await response.Content.ReadFromJsonAsync<List<PreviewQuestionDto>>();
        Assert.NotNull(questions);
        Assert.Equal(10, questions!.Count);
        Assert.All(questions, q => Assert.Contains(q.CorrectAnswer, q.Options));
    }

    [Fact]
    public async Task Create_WithInvalidConfig_Returns400()
    {
        var client = await CreateContentManagerClientAsync();

        var response = await client.PostAsJsonAsync("/api/generator-configs", new
        {
            name = "Rusak " + Guid.NewGuid().ToString("N"),
            category = "Numerical",
            configJson = """{"promptTemplate":"{a}","parameters":[],"operations":[],"answerExpression":"a","distractorExpressions":[],"optionsCount":4}"""
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
