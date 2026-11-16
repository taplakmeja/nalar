using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Psikotes.Application.UseCases.Auth;
using Psikotes.Application.UseCases.Auth.Commands.Register;
using Psikotes.Application.UseCases.Training.Commands.StartSession;
using Psikotes.Application.UseCases.Training.Dtos;
using Xunit;

namespace Psikotes.Web.IntegrationTests;

[Collection("Integration")]
public class TrainingSessionFlowTests
{
    private readonly PsikotesWebApplicationFactory _factory;

    public TrainingSessionFlowTests(PsikotesWebApplicationFactory factory) => _factory = factory;

    private async Task<(HttpClient Client, string AccessToken)> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var request = new RegisterCommand(
            Email: $"session-{Guid.NewGuid():N}@test.local",
            Password: "Password#123",
            DisplayName: "Pengguna Sesi");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return (client, auth.AccessToken);
    }

    private static StartSessionCommand NewStartCommand() => new(
        Mode: "Practice",
        CategoryScope: "Numerical",
        Difficulty: 0,
        QuestionCount: 3,
        ExamProfileSlug: null);

    [Fact]
    public async Task StartAnswerFinishResult_FullFlow_Succeeds()
    {
        var (client, _) = await CreateAuthenticatedClientAsync();

        var startResponse = await client.PostAsJsonAsync("/api/sessions", NewStartCommand());
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);
        var session = await startResponse.Content.ReadFromJsonAsync<StartSessionResponse>();
        Assert.Equal(3, session!.TotalQuestions);

        var question = await client.GetFromJsonAsync<QuestionDto>(
            $"/api/sessions/{session.SessionId}/questions/1");
        Assert.NotEmpty(question!.Prompt);
        Assert.NotEmpty(question.Options);
        Assert.DoesNotContain("\"correctAnswer\"", question.Prompt);

        var repeat = await client.GetFromJsonAsync<QuestionDto>(
            $"/api/sessions/{session.SessionId}/questions/1");
        Assert.Equal(question.Prompt, repeat!.Prompt);

        var answerResponse = await client.PostAsJsonAsync(
            $"/api/sessions/{session.SessionId}/answers",
            new { questionId = question.QuestionId, answer = "jawaban-tebak" });
        Assert.Equal(HttpStatusCode.OK, answerResponse.StatusCode);
        var feedback = await answerResponse.Content.ReadFromJsonAsync<AnswerFeedback>();
        Assert.False(feedback!.IsCorrect);
        Assert.False(string.IsNullOrWhiteSpace(feedback.CorrectAnswer));

        var doubleSubmit = await client.PostAsJsonAsync(
            $"/api/sessions/{session.SessionId}/answers",
            new { questionId = question.QuestionId, answer = "jawaban-tebak" });
        Assert.Equal(HttpStatusCode.BadRequest, doubleSubmit.StatusCode);

        var finishResponse = await client.PostAsync($"/api/sessions/{session.SessionId}/finish", null);
        Assert.Equal(HttpStatusCode.OK, finishResponse.StatusCode);
        var result = await finishResponse.Content.ReadFromJsonAsync<SessionResultDto>();
        Assert.Equal(3, result!.TotalQuestions);
        Assert.Equal(1, result.IncorrectCount);
        Assert.Equal(2, result.UnansweredCount);
        Assert.False(result.IsPassed);
    }

    [Fact]
    public async Task Start_WithoutAuth_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/sessions", NewStartCommand());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Start_WithInvalidMode_ReturnsValidationProblem()
    {
        var (client, _) = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/sessions", new StartSessionCommand(
            Mode: "TidakAda",
            CategoryScope: "Numerical",
            Difficulty: 0,
            QuestionCount: 3,
            ExamProfileSlug: null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ExamProfiles_ListPublicProfiles_ReturnsSixSeeded()
    {
        var (client, _) = await CreateAuthenticatedClientAsync();

        var profiles = await client.GetFromJsonAsync<ExamProfileDto[]>("/api/exam-profiles");

        Assert.NotNull(profiles);
        Assert.True(profiles!.Length >= 6);
        Assert.All(profiles, p => Assert.False(string.IsNullOrWhiteSpace(p.Slug)));
    }

    [Fact]
    public async Task Start_WithExamProfile_OverridesCountAndDeadline()
    {
        var (client, _) = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/sessions", new StartSessionCommand(
            Mode: "ExamSimulation",
            CategoryScope: "Mixed",
            Difficulty: 1,
            QuestionCount: 3,
            ExamProfileSlug: "swasta"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var session = await response.Content.ReadFromJsonAsync<StartSessionResponse>();
        Assert.Equal(30, session!.TotalQuestions);
        Assert.NotNull(session.DeadlineUtc);
    }
}
