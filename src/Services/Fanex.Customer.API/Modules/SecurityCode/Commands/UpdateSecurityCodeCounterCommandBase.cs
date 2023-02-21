namespace Fanex.Customer.API.Modules.SecurityCode.Commands;

public class UpdateSecurityCodeCounterCommandBase
{
    public UpdateSecurityCodeCounterCommandBase(int custId, bool checkedSecurityCode, string language, string ip)
    {
        CustId = custId;
        CheckedSecurityCode = checkedSecurityCode;
        Language = language;
        IP = ip;
    }

    public int CustId { get; }

    public bool CheckedSecurityCode { get; }

    public string Language { get; }

    public string IP { get; }

    public int ErrCode { get; set; } = 0;

    public string ErrMsg { get; set; } = string.Empty;
}