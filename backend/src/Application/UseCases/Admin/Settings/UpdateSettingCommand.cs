using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Admin.Settings;

public sealed record UpdateSettingCommand(string Key, string Value) : ICommand<Result<AppSettingDto>>;
