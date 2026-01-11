namespace ECommerce.Contracts.Common;

public enum ResultType
{
    Success,
    BadRequest,
    NotFound,
    Invalid,
    Conflict,
    Unauthorized,
    Forbidden,
    UnprocessableEntity
}

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public ResultType Type { get; }

    protected Result(bool isSuccess, string error, ResultType type)
    {
        IsSuccess = isSuccess;
        Error = error;
        Type = type;
    }

    public static Result Success() => new(true, string.Empty, ResultType.Success);
    public static Result Failure(string error) => new(false, error, ResultType.BadRequest);
    public static Result NotFound(string error) => new(false, error, ResultType.NotFound);
    public static Result Invalid(string error) => new(false, error, ResultType.Invalid);
    public static Result Conflict(string error) => new(false, error, ResultType.Conflict);
    public static Result Unauthorized(string error) => new(false, error, ResultType.Unauthorized);
    public static Result Unprocessable(string error) => new(false, error, ResultType.UnprocessableEntity);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T? value, bool isSuccess, string error, ResultType type)
        : base(isSuccess, error, type)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty, ResultType.Success);
    public static new Result<T> Failure(string error) => new(default, false, error, ResultType.BadRequest);
    public static new Result<T> NotFound(string error) => new(default, false, error, ResultType.NotFound);
    public static new Result<T> Invalid(string error) => new(default, false, error, ResultType.Invalid);
    public static new Result<T> Conflict(string error) => new(default, false, error, ResultType.Conflict);
    public static new Result<T> Unauthorized(string error) => new(default, false, error, ResultType.Unauthorized);
    public static new Result<T> Unprocessable(string error) => new(default, false, error, ResultType.UnprocessableEntity);

    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (!IsSuccess || Value == null)
        {
            return new Result<TNew>(default, IsSuccess, Error, Type);
        }

        return Result<TNew>.Success(mapper(Value));
    }
}
