using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psikotes.Application.UseCases.Generators.Commands.Create;
using Psikotes.Application.UseCases.Generators.Commands.CreateVersion;
using Psikotes.Application.UseCases.Generators.Commands.Delete;
using Psikotes.Application.UseCases.Generators.Commands.Publish;
using Psikotes.Application.UseCases.Generators.Commands.Rollback;
using Psikotes.Application.UseCases.Generators.Commands.Update;
using Psikotes.Application.UseCases.Generators.Dtos;
using Psikotes.Application.UseCases.Generators.Queries.Get;
using Psikotes.Application.UseCases.Generators.Queries.List;
using Psikotes.Application.UseCases.Generators.Queries.MassTest;
using Psikotes.Application.UseCases.Generators.Queries.Preview;
using Psikotes.Domain.Common;

namespace Psikotes.Web.Controllers.Content;

[ApiController]
[Route("api/generator-configs")]
[Authorize(Roles = $"{Roles.Administrator},{Roles.ContentManager}")]
public sealed class GeneratorConfigsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GeneratorConfigSummaryDto>>> List(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListGeneratorConfigsQuery(), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GeneratorConfigDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGeneratorConfigQuery(id), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost]
    public async Task<ActionResult<GeneratorConfigDto>> Create([FromBody] CreateGeneratorConfigRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateGeneratorConfigCommand(request.Name, request.Category, request.ConfigJson),
            cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GeneratorConfigDto>> Update(Guid id, [FromBody] UpdateGeneratorConfigRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateGeneratorConfigCommand(id, request.Name, request.Category, request.ConfigJson),
            cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/versions")]
    public async Task<ActionResult<GeneratorConfigDto>> CreateVersion(Guid id, [FromBody] CreateVersionRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateGeneratorConfigVersionCommand(id, request.ConfigJson), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteGeneratorConfigCommand(id), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/mass-test")]
    public async Task<ActionResult<MassTestReportDto>> MassTest(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MassTestGeneratorConfigQuery(id), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<GeneratorConfigDto>> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new PublishGeneratorConfigCommand(id), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("rollback/{generatorId}")]
    public async Task<ActionResult<GeneratorConfigDto>> Rollback(string generatorId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RollbackGeneratorConfigCommand(generatorId), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("preview")]
    public async Task<ActionResult<IReadOnlyList<PreviewQuestionDto>>> Preview([FromBody] PreviewRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new PreviewGeneratorConfigQuery(request.ConfigJson, request.Category, request.Samples),
            cancellationToken);
        return result.ToActionResult(this);
    }

    public sealed record CreateGeneratorConfigRequest(string Name, string Category, string ConfigJson);

    public sealed record UpdateGeneratorConfigRequest(string Name, string Category, string ConfigJson);

    public sealed record PreviewRequest(string ConfigJson, string Category, int Samples = 3);

    public sealed record CreateVersionRequest(string ConfigJson);
}
