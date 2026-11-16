namespace Psikotes.Application.UseCases.Training.Dtos;

public sealed record StartSessionResponse(
    Guid SessionId,
    string Mode,
    string CategoryScope,
    int TotalQuestions,
    DateTime? DeadlineUtc);
