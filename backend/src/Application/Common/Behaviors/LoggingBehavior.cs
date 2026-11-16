using Mediator;
using Microsoft.Extensions.Logging;

namespace Psikotes.Application.Common.Behaviors;

public sealed class LoggingBehavior<TMessage, TResponse>(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var messageName = typeof(TMessage).Name;

        logger.LogInformation("Memproses {MessageName}", messageName);

        try
        {
            var response = await next(message, cancellationToken);

            logger.LogInformation("Selesai memproses {MessageName}", messageName);

            return response;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Gagal memproses {MessageName}", messageName);
            throw;
        }
    }
}
