// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace SharedCommon;

public class Result : IResult
{
    internal Result() { }

    internal Result(bool succeeded, string? error)
    {
        Succeeded = succeeded;
        Error = error;
    }

    public bool Succeeded { get; init; }

    //public string[] Errors { get; init; }
    public string? Error { get; init; }

    //public string ErrorMessage => string.Join(", ", Errors??new string[] { });

    public static Result Success()
    {
        return new Result(true, null);
    }
    public static Task<Result> SuccessAsync()
    {
        return Task.FromResult(new Result(true, null));
    }
    public static Result Failure(string? error)
    {
        return new Result(false, error);
    }
    public static Task<Result> FailureAsync(string? error)
    {
        return Task.FromResult(new Result(false, error));
    }
}
public class Result<T> : Result, IResult<T>
{
    public T? Data { get; set; }

    public static new Result<T> Failure(string? error)
    {
        return new Result<T> { Succeeded = false, Error = error };
    }

    public static new async Task<Result<T>> FailureAsync(string? error)
    {
        return await Task.FromResult(Failure(error));
    }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static async Task<Result<T>> SuccessAsync(T data)
    {
        return await Task.FromResult(Success(data));
    }
}

