using Psikotes.Domain.Enums;

namespace Psikotes.Domain.Entities;

public sealed class TrainingSession
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public TrainingMode Mode { get; set; }
    public QuestionCategoryScope CategoryScope { get; set; }
    public int DifficultyScope { get; set; }
    public Guid? ExamProfileId { get; set; }
    public SessionStatus Status { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime? DeadlineUtc { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    public int UnansweredCount { get; set; }
    public decimal Score { get; set; }
    public bool IsPassed { get; set; }

    public ExamProfile? ExamProfile { get; set; }

    public ICollection<SessionQuestion> Questions { get; set; } = new List<SessionQuestion>();
}
