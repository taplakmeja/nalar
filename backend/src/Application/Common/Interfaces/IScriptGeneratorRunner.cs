using Psikotes.QuestionEngine.Abstractions;
using Psikotes.QuestionEngine.Models;

namespace Psikotes.Application.Common.Interfaces;

/// <summary>
/// Seam for executing custom C# script generators. Implementations MUST run
/// scripts out-of-process in an isolated worker (see
/// docs/SCRIPT_GENERATOR_SANDBOX.md); in-process execution is forbidden.
/// </summary>
public interface IScriptGeneratorRunner
{
    Task<GeneratedQuestion> GenerateAsync(
        string generatorId,
        int version,
        Difficulty difficulty,
        long seed,
        CancellationToken cancellationToken = default);
}
