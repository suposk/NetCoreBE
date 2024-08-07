namespace SharedCommon.Interfaces;

public interface IResultCom
{
    //string? ErrorMessage { get; init; }
    string? ErrorMessage { get; }
    bool IsSuccess { get; init; }
    Error? Error { get; init; }
}
public interface IResultCom<out T> : IResultCom
{
    T? Value { get; }
}
