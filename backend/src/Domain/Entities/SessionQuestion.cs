namespace Psikotes.Domain.Entities;

/// <summary>
/// Stores only generation metadata plus the server-side answer key.
/// The rendered question is regenerated on demand from (generator, version,
/// seed, parameters) — never persisted.
/// </summary>
public sealed class SessionQuestion
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public int Order { get; set; }
    public string Category { get; set; } = string.Empty;
    public string GeneratorId { get; set; } = string.Empty;
    public int GeneratorVersion { get; set; }
    public long Seed { get; set; }
    public int Difficulty { get; set; }
    public string ParametersJson { get; set; } = "{}";
    public string CorrectAnswer { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = "[]";
    public string? SubmittedAnswer { get; set; }
    public bool? IsCorrect { get; set; }
    public DateTime? AnsweredAtUtc { get; set; }

    public TrainingSession? Session { get; set; }
}
