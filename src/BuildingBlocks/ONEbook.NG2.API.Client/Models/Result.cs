namespace ONEbook.NG2.API.DTO.CreateAccount;

public class Result : BaseResult
{
    public IList<AccountInfoResult> AccountArray { get; set; } = new List<AccountInfoResult>();
}