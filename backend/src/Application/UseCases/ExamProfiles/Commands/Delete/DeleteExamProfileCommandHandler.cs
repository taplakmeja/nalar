using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Psikotes.Application.Common.Interfaces;

namespace Psikotes.Application.UseCases.ExamProfiles.Commands.Delete;

public sealed class DeleteExamProfileCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<DeleteExamProfileCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteExamProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = await dbContext.ExamProfiles
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (profile is null)
        {
            return Result.NotFound("Profil ujian tidak ditemukan.");
        }

        dbContext.ExamProfiles.Remove(profile);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
