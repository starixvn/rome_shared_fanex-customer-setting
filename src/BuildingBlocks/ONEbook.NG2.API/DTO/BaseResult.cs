namespace ONEbook.NG2.API.DTO.CreateAccount;

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