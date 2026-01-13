namespace TWC.API.Extensions;

public static class ResultExtensions
{
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<IError, T> onFailure)
    {
        if (result.IsSuccess) return onSuccess();

        var error = result.Errors.FirstOrDefault() ?? new Error("Unknown error");

        return onFailure(error);
    }

    public static T Match<T, TValue>(
        this Result<TValue> result,
        Func<T> onSuccess,
        Func<IError, T> onFailure)
    {
        if (result.IsSuccess) return onSuccess();

        var error = result.Errors.FirstOrDefault() ?? new Error("Unknown error");

        return onFailure(error);
    }
}