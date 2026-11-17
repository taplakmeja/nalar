using Ardalis.Result;
using Mediator;

namespace Psikotes.Application.UseCases.Admin.Settings;

public sealed record GetSettingsQuery : IQuery<Result<IReadOnlyList<AppSettingDto>>>;
