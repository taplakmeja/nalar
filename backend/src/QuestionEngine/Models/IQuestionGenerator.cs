using Psikotes.QuestionEngine.Abstractions;

namespace Psikotes.QuestionEngine.Models;

public interface IQuestionGenerator
{
    GeneratorIdentity Identity { get; }

    GeneratedQuestion Generate(Difficulty difficulty, long seed);
}
