using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;
public class CreateNicknameCommand: NonQueryCommand
{
    public CreateNicknameCommand(
        int custId, string nickname, string language, string ip)
    {
        NickName = nickname;
        CustId = custId;
        IP = ip;
        Language = language;
    }

    public string NickName { get; }

    public int CustId { get; }

    public string IP { get; }

    public string Language { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
        => "Age_CustomerInsNickName";

    public override bool IsValid() => CustId > 0;
}