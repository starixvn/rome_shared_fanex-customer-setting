namespace Fanex.Customer.API.Shared.Models;

public class Result
{
    public const int ErrorCode = 1;

    public Result(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; }

    public string Message { get; }

    public bool IsSuccessful => Code == 0;

    public static Result CreateErrorResult(string message, int? code = null)
        => new(code ?? ErrorCode, message);

    public static Result CreateSuccessfulResult(string message = "")
        => new(0, message);
}