using FluentValidation.TestHelper;
using Psikotes.Application.UseCases.Auth.Commands.Refresh;

namespace Psikotes.Application.UnitTests.UseCases.Auth.Commands;

public class RefreshCommandValidatorTests
{
    private readonly RefreshCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new RefreshCommand("token-contoh");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyRefreshToken_FailsValidation(string refreshToken)
    {
        var command = new RefreshCommand(refreshToken);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.RefreshToken);
    }
}
