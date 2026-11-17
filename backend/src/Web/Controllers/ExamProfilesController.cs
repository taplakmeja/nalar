using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.ExamProfiles.Commands.Create;
using Psikotes.Application.UseCases.ExamProfiles.Commands.Delete;
using Psikotes.Application.UseCases.ExamProfiles.Commands.Update;
using Psikotes.Application.UseCases.ExamProfiles.Dtos;
using Psikotes.Application.UseCases.ExamProfiles.Queries.ListAll;
using Psikotes.Application.UseCases.Training.Dtos;
using Psikotes.Application.UseCases.Training.Queries.ListExamProfiles;
using Psikotes.Domain.Common;

namespace Psikotes.Web.Controllers;

[ApiController]
[Route("api/exam-profiles")]
public sealed class ExamProfilesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ExamProfileDto>>> List(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListExamProfilesQuery(), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("admin")]
    [Authorize(Roles = $"{Roles.Administrator},{Roles.ContentManager}")]
    public async Task<ActionResult<IReadOnlyList<AdminExamProfileDto>>> ListAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListAllExamProfilesQuery(), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Administrator},{Roles.ContentManager}")]
    public async Task<ActionResult<AdminExamProfileDto>> Create([FromBody] CreateExamProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateExamProfileCommand(
                request.Slug,
                request.Name,
                request.Description,
                request.CategoryWeightsJson,
                request.QuestionCount,
                request.DurationMinutes,
                request.PassingScorePercent,
                request.IsPublic),
            cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Administrator},{Roles.ContentManager}")]
    public async Task<ActionResult<AdminExamProfileDto>> Update(Guid id, [FromBody] UpdateExamProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateExamProfileCommand(
                id,
                request.Name,
                request.Description,
                request.CategoryWeightsJson,
                request.QuestionCount,
                request.DurationMinutes,
                request.PassingScorePercent,
                request.IsPublic),
            cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.Administrator},{Roles.ContentManager}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteExamProfileCommand(id), cancellationToken);
        return result.ToActionResult(this);
    }

    public sealed record CreateExamProfileRequest(
        string Slug,
        string Name,
        string Description,
        string CategoryWeightsJson,
        int QuestionCount,
        int DurationMinutes,
        decimal PassingScorePercent,
        bool IsPublic);

    public sealed record UpdateExamProfileRequest(
        string Name,
        string Description,
        string CategoryWeightsJson,
        int QuestionCount,
        int DurationMinutes,
        decimal PassingScorePercent,
        bool IsPublic);
}
