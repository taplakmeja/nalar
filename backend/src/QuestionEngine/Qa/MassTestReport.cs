namespace Psikotes.QuestionEngine.Qa;

public sealed record MassTestReport(
    string GeneratorId,
    int GeneratorVersion,
    int TotalQuestions,
    int DistinctQuestions,
    int DuplicateRate,
    int InvalidCount,
    bool Deterministic,
    IReadOnlyList<string> Issues);
