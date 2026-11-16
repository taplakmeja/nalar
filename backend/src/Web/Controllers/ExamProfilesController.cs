using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Psikotes.Application.UseCases.Training.Dtos;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Training.Queries.ListExamProfiles;

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
}
