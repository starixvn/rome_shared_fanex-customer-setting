namespace Fanex.Customer.API.Shared.Models;

public class Result
{
    public const int ErrorCode = 1;

    public Result(int code, string message, string customData = null)
    {
        Code = code;
        Message = message;
        CustomData = customData;
    }

    public int Code { get; }

    public string Message { get; set; }

    public string CustomData { get; }

    public bool IsSuccessful => Code == 0;

    public static Result CreateErrorResult(string message, int? code = null, string customData = null)
        => new(code ?? ErrorCode, message, customData);

    public static Result CreateSuccessfulResult(string message = "")
        => new(0, message);
}