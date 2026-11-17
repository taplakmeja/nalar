using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Psikotes.Application.Common.Interfaces;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.Application.UseCases.Generators.Dtos;

namespace Psikotes.Application.UseCases.Generators.Queries.Preview;

public sealed class PreviewGeneratorConfigQueryHandler(IRuleGeneratorFactory factory)
    : IQueryHandler<PreviewGeneratorConfigQuery, Result<IReadOnlyList<PreviewQuestionDto>>>
{
    public ValueTask<Result<IReadOnlyList<PreviewQuestionDto>>> Handle(PreviewGeneratorConfigQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(query.ConfigJson);

        var generator = factory.Create("preview", 1, query.Category, query.ConfigJson, out var issues);
        if (issues.Count > 0)
        {
            return ValueTask.FromResult(
                Result<IReadOnlyList<PreviewQuestionDto>>
                    .Error(new ErrorList([string.Join(" ", issues)], null)));
        }

        var samples = new List<PreviewQuestionDto>();
        var seed = 1000L;
        for (var difficulty = 0; difficulty <= 4; difficulty++)
        {
            for (var i = 0; i < query.Samples; i++)
            {
                var question = generator.Generate((Difficulty)difficulty, seed++);
                samples.Add(new PreviewQuestionDto(
                    difficulty,
                    question.Seed,
                    question.Prompt,
                    question.CorrectAnswer,
                    question.Options));
            }
        }

        return ValueTask.FromResult(Result.Success<IReadOnlyList<PreviewQuestionDto>>(samples));
    }
}
