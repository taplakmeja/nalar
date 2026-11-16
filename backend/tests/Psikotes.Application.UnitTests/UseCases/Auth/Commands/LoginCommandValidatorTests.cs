using FluentValidation.TestHelper;
using Psikotes.Application.UseCases.Auth.Commands.Login;

namespace Psikotes.Application.UnitTests.UseCases.Auth.Commands;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new LoginCommand("pengguna@example.com", "Rahasia#123");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyEmail_FailsValidation(string email)
    {
        var command = new LoginCommand(email, "Rahasia#123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Fact]
    public void EmptyPassword_FailsValidation()
    {
        var command = new LoginCommand("pengguna@example.com", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}
