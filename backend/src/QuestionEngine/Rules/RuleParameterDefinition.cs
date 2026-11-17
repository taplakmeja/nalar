namespace Psikotes.QuestionEngine.Rules;

/// <summary>
/// A randomized parameter. Min/Max apply to Beginner; MaxByDifficulty
/// (optional, 5 entries ascending difficulty) overrides Max per level.
/// </summary>
public sealed record RuleParameterDefinition(
    string Name,
    double Min,
    double Max,
    bool Integer = true,
    double[]? MaxByDifficulty = null);
