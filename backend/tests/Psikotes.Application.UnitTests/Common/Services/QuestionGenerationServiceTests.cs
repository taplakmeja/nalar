using Psikotes.Application.Common.Services;
using Psikotes.QuestionEngine;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.Application.UnitTests.Common.Services;

public class QuestionGenerationServiceTests
{
    private static QuestionGenerationService CreateService()
    {
        var registry = new GeneratorRegistry(
            typeof(QuestionEngine.Models.IQuestionGenerator).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(QuestionEngineMarker)
                .Select(t => (IQuestionGenerator)Activator.CreateInstance(t)!)
                .ToList());
        return new QuestionGenerationService(registry);
    }

    private static bool QuestionEngineMarker(Type t) =>
        typeof(IQuestionGenerator).IsAssignableFrom(t) &&
        t.Namespace is not null &&
        t.Namespace.StartsWith("Psikotes.QuestionEngine");

    [Fact]
    public void Generate_Numerical_ReturnsValidQuestion()
    {
        var service = CreateService();

        var question = service.Generate("Numerical", 1, seed: 42);

        Assert.NotEmpty(question.Prompt);
        Assert.NotEmpty(question.CorrectAnswer);
        Assert.Contains(question.CorrectAnswer, question.Options);
    }

    [Fact]
    public void Generate_SameSeed_IsDeterministic()
    {
        var service = CreateService();

        var first = service.Generate("Numerical", 2, seed: 1234);
        var second = service.Generate("Numerical", 2, seed: 1234);

        Assert.Equal(first.Prompt, second.Prompt);
        Assert.Equal(first.CorrectAnswer, second.CorrectAnswer);
    }

    [Fact]
    public void Generate_UnknownCategory_Throws()
    {
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.Generate("Nonexistent", 1, 1));
    }
}
