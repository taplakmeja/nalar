using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;

namespace Psikotes.Application.UseCases.Admin.Settings;

public sealed class GetSettingsQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetSettingsQuery, Result<IReadOnlyList<AppSettingDto>>>
{
    public async ValueTask<Result<IReadOnlyList<AppSettingDto>>> Handle(GetSettingsQuery query, CancellationToken cancellationToken)
    {
        var settings = await dbContext.AppSettings
            .AsNoTracking()
            .OrderBy(s => s.Key)
            .Select(s => new AppSettingDto(s.Key, s.Value, s.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AppSettingDto>>(settings);
    }
}
