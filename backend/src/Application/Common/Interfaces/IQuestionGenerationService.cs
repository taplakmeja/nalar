using Psikotes.QuestionEngine.Models;

namespace Psikotes.Application.Common.Interfaces;

public interface IQuestionGenerationService
{
    GeneratedQuestion Generate(string category, int difficulty, long seed);
}
