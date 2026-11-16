using FluentValidation.TestHelper;
using Psikotes.Application.UseCases.Training.Commands.StartSession;

namespace Psikotes.Application.UnitTests.UseCases.Training.Commands;

public class StartSessionCommandValidatorTests
{
    private readonly StartSessionCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new StartSessionCommand("Practice", "Mixed", 1, 10, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("BukanMode")]
    [InlineData("")]
    public void InvalidMode_FailsValidation(string mode)
    {
        var command = new StartSessionCommand(mode, "Mixed", 1, 10, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Mode);
    }

    [Fact]
    public void QuestionCountOutOfRange_FailsValidation()
    {
        var command = new StartSessionCommand("Practice", "Mixed", 1, 500, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.QuestionCount);
    }

    [Fact]
    public void DifficultyOutOfRange_FailsValidation()
    {
        var command = new StartSessionCommand("Practice", "Mixed", 9, 10, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Difficulty);
    }
}
