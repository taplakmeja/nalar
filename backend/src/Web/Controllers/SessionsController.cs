using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Training.Commands.FinishSession;
using Psikotes.Application.UseCases.Training.Dtos;
using Psikotes.Application.UseCases.Training.Commands.StartSession;
using Psikotes.Application.UseCases.Training.Commands.SubmitAnswer;
using Psikotes.Application.UseCases.Training.Queries.GetSessionQuestion;
using Psikotes.Application.UseCases.Training.Queries.GetSessionResult;

namespace Psikotes.Web.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
public sealed class SessionsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<StartSessionResponse>> Start([FromBody] StartSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("{sessionId:guid}/questions/{order:int}")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(Guid sessionId, int order, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSessionQuestionQuery(sessionId, order), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("{sessionId:guid}/answers")]
    public async Task<ActionResult<AnswerFeedback>> SubmitAnswer(Guid sessionId, [FromBody] SubmitAnswerRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SubmitAnswerCommand(sessionId, request.QuestionId, request.Answer), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("{sessionId:guid}/finish")]
    public async Task<ActionResult<SessionResultDto>> Finish(Guid sessionId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new FinishSessionCommand(sessionId), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("{sessionId:guid}/result")]
    public async Task<ActionResult<SessionResultDto>> GetResult(Guid sessionId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSessionResultQuery(sessionId), cancellationToken);
        return result.ToActionResult(this);
    }
}

public sealed record SubmitAnswerRequest(Guid QuestionId, string Answer);
