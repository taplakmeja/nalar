using FluentValidation.TestHelper;
using Psikotes.Application.UseCases.Auth.Commands.Register;

namespace Psikotes.Application.UnitTests.UseCases.Auth.Commands;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var command = new RegisterCommand("pengguna@example.com", "Rahasia#123", "Pengguna");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("bukan-email")]
    [InlineData("tanpa-at.com")]
    public void InvalidEmail_FailsValidation(string email)
    {
        var command = new RegisterCommand(email, "Rahasia#123", "Pengguna");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Theory]
    [InlineData("Pendek1")]
    [InlineData("tanpaangkaAa")]
    [InlineData("TANPAKECIL123")]
    [InlineData("tanpabesar123")]
    public void WeakPassword_FailsValidation(string password)
    {
        var command = new RegisterCommand("pengguna@example.com", password, "Pengguna");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyDisplayName_FailsValidation(string displayName)
    {
        var command = new RegisterCommand("pengguna@example.com", "Rahasia#123", displayName);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.DisplayName);
    }

    [Fact]
    public void DisplayName_LongerThan100Characters_FailsValidation()
    {
        var command = new RegisterCommand("pengguna@example.com", "Rahasia#123", new string('a', 101));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.DisplayName);
    }
}
