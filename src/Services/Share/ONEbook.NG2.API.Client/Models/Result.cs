namespace ONEbook.NG2.API.Client.Models;

public class Result : BaseResult
{
    public IList<AccountInfoResult> AccountArray { get; set; } = new List<AccountInfoResult>();
}