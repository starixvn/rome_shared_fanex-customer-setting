namespace Alpha.CloneAccount.Models;

public class Result
{
    public Result()
        : this(0, "")
    {
    }

    public Result(int code, string message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; }

    public string Message { get; set; }

    public bool IsSuccess => Code == 0;
}

