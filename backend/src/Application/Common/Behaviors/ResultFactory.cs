using Ardalis.Result;
using System.Reflection;

namespace Psikotes.Application.Common.Behaviors;

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
