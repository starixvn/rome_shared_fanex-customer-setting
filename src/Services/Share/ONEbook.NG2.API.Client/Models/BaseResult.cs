namespace ONEbook.NG2.API.Client.Models;

public class BaseResult : BaseToken
{
    public int ErrorCode { get; set; }

    public string ErrorDescription { get; set; }

    public bool IsSuccess
    {
        get
        {
            return ErrorCode == 0;
        }
    }
}