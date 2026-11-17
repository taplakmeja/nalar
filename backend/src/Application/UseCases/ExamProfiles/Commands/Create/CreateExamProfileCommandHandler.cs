using Ardalis.GuardClauses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;
using Psikotes.Application.UseCases.ExamProfiles.Dtos;
using Psikotes.Domain.Entities;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Create;

public sealed class CreateExamProfileCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<CreateExamProfileCommand, Result<AdminExamProfileDto>>
{
    public async ValueTask<Result<AdminExamProfileDto>> Handle(CreateExamProfileCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrWhiteSpace(command.Slug);
        Guard.Against.NullOrWhiteSpace(command.Name);

        var slugExists = await dbContext.ExamProfiles
            .AnyAsync(p => p.Slug == command.Slug, cancellationToken);
        if (slugExists)
        {
            return Result.Invalid(new ValidationError($"Profil ujian dengan slug '{command.Slug}' sudah ada."));
        }

        var profile = new ExamProfile
        {
            Id = Guid.NewGuid(),
            Slug = command.Slug,
            Name = command.Name.Trim(),
            Description = command.Description,
            CategoryWeightsJson = command.CategoryWeightsJson,
            QuestionCount = command.QuestionCount,
            DurationMinutes = command.DurationMinutes,
            PassingScorePercent = command.PassingScorePercent,
            IsPublic = command.IsPublic
        };

        dbContext.ExamProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(profile));
    }

    internal static AdminExamProfileDto ToDto(ExamProfile profile) =>
        new(profile.Id,
            profile.Slug,
            profile.Name,
            profile.Description,
            profile.CategoryWeightsJson,
            profile.QuestionCount,
            profile.DurationMinutes,
            profile.PassingScorePercent,
            profile.IsPublic);
}
