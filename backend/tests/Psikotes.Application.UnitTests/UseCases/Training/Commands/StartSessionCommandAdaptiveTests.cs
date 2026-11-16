using FluentValidation.TestHelper;
using Psikotes.Application.UseCases.Training.Commands.StartSession;

namespace Psikotes.Application.UnitTests.UseCases.Training.Commands;

public class StartSessionCommandAdaptiveTests
{
    private readonly StartSessionCommandValidator _validator = new();

    [Fact]
    public void AdaptiveFlag_WithValidCommand_PassesValidation()
    {
        var command = new StartSessionCommand("WeaknessTraining", "Mixed", 2, 10, null, UseAdaptiveDifficulty: true);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
