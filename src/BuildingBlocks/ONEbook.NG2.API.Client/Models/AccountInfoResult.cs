namespace ONEbook.NG2.API.Client.Models;

public class AccountInfoResult : BaseResult
{
    public int ObCustId { get; set; }

    public int IsCreated { get; set; }

    public new bool IsSuccess
    {
        get
        {
            return IsCreated == 1;
        }
    }
}