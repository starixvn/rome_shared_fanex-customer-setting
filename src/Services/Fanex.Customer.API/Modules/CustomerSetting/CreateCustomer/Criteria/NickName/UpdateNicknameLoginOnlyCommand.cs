using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;
public class UpdateNicknameLoginOnlyCommand : NonQueryCommand
{
    public UpdateNicknameLoginOnlyCommand(
        int custId,
        string language,
        string ip)
    {
        CustId = custId;
        IP = ip;
        Language = language;
    }

    public bool NickNameLoginOnly => true;

    public int CustId { get; }

    public string IP { get; }

    public string Language { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
        => "Age_CustomerUpdNicknameLoginOnly";

    public override bool IsValid() => CustId > 0;
}