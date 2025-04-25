namespace CashService.API.Models;

public class Result
{
    public int ErrCode { get; set; }
    public string ErrMsg { get; set; } = string.Empty;

    public static Result Success => new Result();
}
