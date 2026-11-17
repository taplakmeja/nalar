using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.ExamProfiles.Commands.Create;
using Psikotes.Application.UseCases.ExamProfiles.Dtos;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Update;

public sealed class UpdateExamProfileCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<UpdateExamProfileCommand, Result<AdminExamProfileDto>>
{
    public async ValueTask<Result<AdminExamProfileDto>> Handle(UpdateExamProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = await dbContext.ExamProfiles
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (profile is null)
        {
            return Result.NotFound("Profil ujian tidak ditemukan.");
        }

        profile.Name = command.Name.Trim();
        profile.Description = command.Description;
        profile.CategoryWeightsJson = command.CategoryWeightsJson;
        profile.QuestionCount = command.QuestionCount;
        profile.DurationMinutes = command.DurationMinutes;
        profile.PassingScorePercent = command.PassingScorePercent;
        profile.IsPublic = command.IsPublic;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(CreateExamProfileCommandHandler.ToDto(profile));
    }
}
