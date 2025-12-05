namespace SocialNetwork.API.Models;
// Generic Result class for 


public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    protected Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string message) => new(false, message);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccess, T? data, string? errorMessage)
        : base(isSuccess, errorMessage)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public new static Result<T> Failure(string message) => new(false, default, message);
}