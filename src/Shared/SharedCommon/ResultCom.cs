using System.Net;

namespace SharedCommon;

public class ResultCom : IResultCom
{
    internal ResultCom() { }

    public ResultCom(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException($"must provide Error.None");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException($"must provide {nameof(error)}");

        IsSuccess = isSuccess;
        Error = error;
    }

    public ResultCom(bool isSuccess, string? errorMessage, HttpStatusCode? statusCode = null)
    {
        if (isSuccess && !string.IsNullOrWhiteSpace(errorMessage))        
            throw new InvalidOperationException($"{nameof(errorMessage)} should not be provide");

        if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))        
            throw new InvalidOperationException($"must provide {nameof(errorMessage)}");
        
        IsSuccess = isSuccess;
        Error = new Error(errorMessage, statusCode);
    }

    public bool IsSuccess { get; init; }
    public Error? Error { get; init; }

    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage => Error?.Message;
    public static ResultCom Success() => new(true, Error.None);
    public static Task<ResultCom> SuccessAsync() => Task.FromResult(new ResultCom(true, Error.None));    
    public static ResultCom Failure(string? errorMessage, HttpStatusCode? statusCode = null) => new(false, errorMessage, statusCode);
    public static Task<ResultCom> FailureAsync(string? errorMessage, HttpStatusCode? statusCode = null) => Task.FromResult(new ResultCom(false, errorMessage, statusCode));    
}

public class ResultCom<T> : ResultCom, IResultCom<T>
{
    public T? Value { get; set; }

    public static new ResultCom<T> Failure(string? errorMessage, HttpStatusCode? statusCode = null) => new ResultCom<T> { IsSuccess = false, Error = new Error(errorMessage, statusCode) };    
    public static new async Task<ResultCom<T>> FailureAsync(string? errorMessage, HttpStatusCode? statusCode = null) => await Task.FromResult(Failure(errorMessage, statusCode));    
    public static ResultCom<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static async Task<ResultCom<T>> SuccessAsync(T value) => await Task.FromResult(Success(value));    
}


public class Error
{
    public string? Message { get; init; }
    public int? StatusCode => HttpStatusCode.HasValue ? (int)HttpStatusCode.Value : null;
    public HttpStatusCode? HttpStatusCode {get; init;}

    public Error(string? message)
    {
        Message = message ??= string.Empty;                
    }

    public Error(string? message, HttpStatusCode? statusCode)
    {
        Message = message ??= string.Empty;
        HttpStatusCode  = statusCode;        
    }

    public static readonly Error None = new(string.Empty, null);
}
