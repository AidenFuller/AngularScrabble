namespace Scrabble.Server.Models;

public class Result
{
    public string? Message { get; }
    public bool IsSuccess => Message is null;

    protected Result(string? message)
    {
        Message = message;
    }

    public static Result Ok() => new(null);
    public static Result<T> Ok<T>(T value) => new(value, null);
    public static Result Fail(string message) => new(message);
    public static Result<T> Fail<T>(string message) => new Result<T>(default, message);
}

public class Result<T> : Result
{
    public T? Value { get; }

    public Result(T? value, string? message) : base(message)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(value, null);
    public new static Result<T> Fail(string message) => new(default, message);
}
