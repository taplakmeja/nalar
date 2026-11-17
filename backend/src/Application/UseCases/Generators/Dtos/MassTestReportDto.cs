namespace Psikotes.Application.UseCases.Generators.Dtos;

public sealed record MassTestReportDto(
    string GeneratorId,
    int GeneratorVersion,
    int TotalQuestions,
    int DistinctQuestions,
    int DuplicateRate,
    int InvalidCount,
    bool Deterministic,
    IReadOnlyList<string> Issues,
    bool Passed);
