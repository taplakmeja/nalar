namespace Psikotes.Domain.Entities;

public sealed class ExamProfile
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryWeightsJson { get; set; } = "{}";
    public int QuestionCount { get; set; }
    public int DurationMinutes { get; set; }
    public decimal PassingScorePercent { get; set; }
    public bool IsPublic { get; set; } = true;
}
