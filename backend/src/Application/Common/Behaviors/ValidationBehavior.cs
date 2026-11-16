using Ardalis.Result;
using FluentValidation;
using Mediator;
using System.Reflection;

namespace Psikotes.Application.Common.Behaviors;

public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IBaseRequest
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var context = new ValidationContext<TMessage>(message);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(message, cancellationToken);
        }

        var validationErrors = failures
            .Select(f => new ValidationError
            {
                Identifier = f.PropertyName,
                ErrorMessage = f.ErrorMessage
            })
            .ToList();

        if (ResultFactory<TResponse>.TryCreateInvalid(validationErrors, out var invalidResponse))
        {
            return invalidResponse;
        }

        throw new ValidationException(failures);
    }
}
