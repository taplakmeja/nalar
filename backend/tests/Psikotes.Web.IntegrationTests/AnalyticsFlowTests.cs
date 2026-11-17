using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Psikotes.Application.UseCases.Auth;
using Psikotes.Application.UseCases.Analytics.Dtos;
using Psikotes.Application.UseCases.Auth.Commands.Register;
using Psikotes.Application.UseCases.Training.Commands.StartSession;
using Psikotes.Application.UseCases.Training.Dtos;
using Xunit;

namespace Psikotes.Web.IntegrationTests;

[Collection("Integration")]
public class AnalyticsFlowTests
{
    private readonly PsikotesWebApplicationFactory _factory;

    public AnalyticsFlowTests(PsikotesWebApplicationFactory factory) => _factory = factory;

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var request = new RegisterCommand(
            Email: $"analytics-{Guid.NewGuid():N}@test.local",
            Password: "Password#123",
            DisplayName: "Pengguna Analitik");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return client;
    }

    private static async Task CompletePracticeSessionAsync(HttpClient client, int questionCount)
    {
        var startResponse = await client.PostAsJsonAsync("/api/sessions", new StartSessionCommand(
            Mode: "Practice",
            CategoryScope: "Numerical",
            Difficulty: 0,
            QuestionCount: questionCount,
            ExamProfileSlug: null));
        startResponse.EnsureSuccessStatusCode();
        var session = await startResponse.Content.ReadFromJsonAsync<StartSessionResponse>();

        for (var order = 1; order <= session!.TotalQuestions; order++)
        {
            var question = await client.GetFromJsonAsync<QuestionDto>(
                $"/api/sessions/{session.SessionId}/questions/{order}");
            await client.PostAsJsonAsync(
                $"/api/sessions/{session.SessionId}/answers",
                new { questionId = question!.QuestionId, answer = "tebakan-salah" });
        }

        var finish = await client.PostAsync($"/api/sessions/{session.SessionId}/finish", null);
        finish.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Dashboard_WithoutHistory_ReturnsIndicativeReadiness()
    {
        var client = await CreateAuthenticatedClientAsync();

        var dashboard = await client.GetFromJsonAsync<DashboardDto>("/api/analytics/dashboard");

        Assert.NotNull(dashboard!);
        Assert.Equal(0, dashboard.SessionsCompleted);
        Assert.Equal(0, dashboard.Readiness.Percent);
        Assert.Equal("Data Belum Cukup", dashboard.Readiness.Label);
        Assert.Contains("indikatif", dashboard.Readiness.Disclaimer, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("bukan jaminan", dashboard.Readiness.Disclaimer);
    }

    [Fact]
    public async Task Dashboard_AfterAnsweredSessions_ComputesAccuracyAndRecommendations()
    {
        var client = await CreateAuthenticatedClientAsync();
        await CompletePracticeSessionAsync(client, questionCount: 5);

        var dashboard = await client.GetFromJsonAsync<DashboardDto>("/api/analytics/dashboard");

        Assert.Equal(1, dashboard!.SessionsCompleted);
        Assert.Equal(5, dashboard.TotalAnswered);
        Assert.Equal(0, dashboard.AccuracyPercent);
        var numerical = dashboard.Categories.SingleOrDefault(c => c.Category == "Numerical");
        Assert.NotNull(numerical);
        Assert.Equal(5, numerical!.Answered);
        Assert.Equal(0, numerical.Correct);
        Assert.Equal(0, numerical.ConsecutiveCorrect);
        Assert.Equal(5, numerical.ConsecutiveIncorrect);
        Assert.Contains(dashboard.Recommendations, r => r.Category == "Numerical");
    }

    [Fact]
    public async Task WeaknessTraining_WithMixedScope_AutoSelectsWeakestCategory()
    {
        var client = await CreateAuthenticatedClientAsync();
        await CompletePracticeSessionAsync(client, questionCount: 5);

        var startResponse = await client.PostAsJsonAsync("/api/sessions", new StartSessionCommand(
            Mode: "WeaknessTraining",
            CategoryScope: "Mixed",
            Difficulty: 0,
            QuestionCount: 3,
            ExamProfileSlug: null));
        startResponse.EnsureSuccessStatusCode();

        var session = await startResponse.Content.ReadFromJsonAsync<StartSessionResponse>();
        Assert.Equal("Numerical", session!.CategoryScope);
    }

    [Fact]
    public async Task Dashboard_WithoutAuth_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/analytics/dashboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
