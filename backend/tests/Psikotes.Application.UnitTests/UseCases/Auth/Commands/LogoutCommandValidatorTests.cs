using FluentValidation.TestHelper;
using Psikotes.Application.UseCases.Auth.Commands.Logout;

namespace Psikotes.Application.UnitTests.UseCases.Auth.Commands;

public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new LogoutCommand("token-contoh");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyRefreshToken_FailsValidation(string refreshToken)
    {
        var command = new LogoutCommand(refreshToken);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.RefreshToken);
    }
}
