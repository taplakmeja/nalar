using Psikotes.QuestionEngine;
using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Generators.Abstract;
using Psikotes.QuestionEngine.Generators.Kraepelin;
using Psikotes.QuestionEngine.Generators.Logical;
using Psikotes.QuestionEngine.Generators.Numerical;
using Psikotes.QuestionEngine.Generators.Spatial;
using Psikotes.QuestionEngine.Generators.Verbal;
using Psikotes.QuestionEngine.Models;
using Psikotes.QuestionEngine.Qa;
using Psikotes.QuestionEngine.Validation;
using Psikotes.QuestionEngine.Datasets;
using Xunit;

namespace Psikotes.QuestionEngine.Tests;

public class CategorySweepTests
{
    public static TheoryData<IQuestionGenerator> AllGenerators() => new()
    {
        new ArithmeticAdditionGenerator(),
        new NumberSequenceGenerator(),
        new FractionGenerator(),
        new PercentageGenerator(),
        new RatioProportionGenerator(),
        new WordProblemGenerator(),
        new SynonymGenerator(),
        new AntonymGenerator(),
        new AnalogyGenerator(),
        new VocabularyClassificationGenerator(),
        new SyllogismGenerator(),
        new LogicalSequenceGenerator(),
        new StatementConclusionGenerator(),
        new ShapeSequenceGenerator(),
        new MissingFigureGenerator(),
        new CubeCountGenerator(),
        new RotationMatchGenerator(),
        new PaperFoldingGenerator()
    };

    [Theory]
    [MemberData(nameof(AllGenerators))]
    public void Generator_IsDeterministic_AndTechnicallyValid(IQuestionGenerator generator)
    {
        Assert.True(DeterminismHarness.VerifyAll(generator, samplesPerDifficulty: 10),
            $"{generator.Identity.Id} is not deterministic.");

        foreach (var difficulty in Enum.GetValues<Difficulty>())
        {
            for (var seed = 1L; seed <= 25; seed++)
            {
                var question = generator.Generate(difficulty, seed * 31);
                Assert.Equal(ValidationStatus.Valid, TechnicalValidator.Validate(question));
            }
        }
    }

    [Theory]
    [MemberData(nameof(AllGenerators))]
    public void Generator_MassTest_HasNoInvalidQuestions(IQuestionGenerator generator)
    {
        var report = MassTester.Run(generator, samplesPerDifficulty: 20);

        Assert.Equal(0, report.InvalidCount);
        Assert.True(report.Deterministic);
        Assert.Empty(report.Issues.Where(i => i.Contains("technical validation")));
    }

    [Fact]
    public void VerbalDataset_IsVersioned_AndPopulated()
    {
        var dataset = VerbalDatasetProvider.Dataset;

        Assert.Equal(1, dataset.Version);
        Assert.True(dataset.Synonyms.Count >= 20);
        Assert.True(dataset.Antonyms.Count >= 20);
        Assert.True(dataset.Analogies.Count >= 15);
        Assert.True(dataset.Classifications.Count >= 10);
    }

    [Fact]
    public void KraepelinGrid_IsDeterministic()
    {
        var generator = new KraepelinGridGenerator();

        var first = generator.Generate(rows: 10, columns: 8, seed: 42, durationMinutes: 5);
        var second = generator.Generate(rows: 10, columns: 8, seed: 42, durationMinutes: 5);

        Assert.Equal(80, first.Cells.Count);
        Assert.Equal(first.Cells, second.Cells);
        Assert.Equal(5, first.DurationMinutes);
    }
}
