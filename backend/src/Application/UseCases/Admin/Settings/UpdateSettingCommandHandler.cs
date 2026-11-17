using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.Admin.Settings;

public sealed class UpdateSettingCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<UpdateSettingCommand, Result<AppSettingDto>>
{
    public async ValueTask<Result<AppSettingDto>> Handle(UpdateSettingCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Key);

        var setting = await dbContext.AppSettings
            .FirstOrDefaultAsync(s => s.Key == command.Key, cancellationToken);

        if (setting is null)
        {
            setting = new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = command.Key,
                Value = command.Value,
                UpdatedAtUtc = DateTime.UtcNow
            };
            dbContext.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = command.Value;
            setting.UpdatedAtUtc = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AppSettingDto(setting.Key, setting.Value, setting.UpdatedAtUtc));
    }
}
