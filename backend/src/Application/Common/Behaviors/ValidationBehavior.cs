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

internal static class ResultFactory<TResponse>
{
    private static readonly Func<List<ValidationError>, TResponse>? _invalidFactory = BuildInvalidFactory();

    public static bool TryCreateInvalid(List<ValidationError> errors, out TResponse response)
    {
        if (_invalidFactory is null)
        {
            response = default!;
            return false;
        }

        response = _invalidFactory(errors);
        return true;
    }

    private static Func<List<ValidationError>, TResponse>? BuildInvalidFactory()
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return errors => (TResponse)(object)Result.Invalid((IEnumerable<ValidationError>)errors);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var method = responseType.GetMethod(
                nameof(Result.Invalid),
                BindingFlags.Public | BindingFlags.Static,
                [typeof(IEnumerable<ValidationError>)]);

            if (method is null)
            {
                return null;
            }

            return errors => (TResponse)method.Invoke(null, [errors])!;
        }

        return null;
    }
}
