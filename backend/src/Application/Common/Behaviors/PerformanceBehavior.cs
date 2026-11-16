using Mediator;
using Microsoft.Extensions.Logging;

namespace Psikotes.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TMessage, TResponse>(ILogger<PerformanceBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private const long SlowThresholdMilliseconds = 500;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();

        var response = await next(message, cancellationToken);

        timer.Stop();

        if (timer.ElapsedMilliseconds > SlowThresholdMilliseconds)
        {
            logger.LogWarning(
                "{MessageName} butuh {ElapsedMilliseconds} ms (melebihi ambang {Threshold} ms)",
                typeof(TMessage).Name,
                timer.ElapsedMilliseconds,
                SlowThresholdMilliseconds);
        }

        return response;
    }
}
