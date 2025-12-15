namespace HrManager.Domain.Primitives;

public class Result
{
    protected Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToArray();
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string[] Errors { get; }

    public static Result Success() => new(true, Array.Empty<string>());

    public static Result Failure(params string[] errors) => new(false, errors);

    public static Result<T> Success<T>(T value) => new(value, true, Array.Empty<string>());

    public static Result<T> Failure<T>(params string[] errors) => new(default!, false, errors);
}

public sealed class Result<T> : Result
{
    internal Result(T value, bool isSuccess, IEnumerable<string> errors) : base(isSuccess, errors)
    {
        Value = value;
    }

    public T Value { get; }
}
