namespace Psikotes.QuestionEngine.Abstractions;

public enum QuestionCategory
{
    Numerical,
    Verbal,
    Logical,
    Abstract,
    Spatial,
    Kraepelin
}

public enum Difficulty
{
    Beginner,
    Easy,
    Intermediate,
    Advanced,
    Expert
}

public enum ValidationStatus
{
    Pending,
    Valid,
    Invalid
}
