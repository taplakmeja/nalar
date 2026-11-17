namespace Psikotes.Application.UseCases.Admin.Settings;

public sealed record AppSettingDto(string Key, string Value, DateTime UpdatedAtUtc);
