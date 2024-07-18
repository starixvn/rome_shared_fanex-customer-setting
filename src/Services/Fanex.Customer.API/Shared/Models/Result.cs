namespace Fanex.Customer.API.Shared.Models;

public class Result
{
    public const int ErrorCode = 1;

    public Result()
    {
    }

    public Result(int code, string message, string customData = null)
    {
        Code = code;
        Message = message;
        CustomData = customData;
    }

    public int Code { get; set; }

    public string Message { get; set; }

    public string CustomData { get; }

    public bool IsSuccessful => Code == 0;

    public static Result Create(string message, int? code = null, string customData = null)
        => new(code ?? ErrorCode, message, customData);

    public static Result CreateSuccess(string message = "")
        => new(0, message);
}