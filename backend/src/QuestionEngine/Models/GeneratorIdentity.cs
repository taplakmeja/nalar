using Psikotes.QuestionEngine.Abstractions;

namespace Psikotes.QuestionEngine.Models;

public sealed record GeneratorIdentity(string Id, int Version, QuestionCategory Category);
