using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class RegisterNicknameCommand(int custId, string nickname, string language) : NonQueryCommand
{
    public int CustId => custId;

    public string Nickname => nickname;

    public string Language => language;

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_CustomerInsNickName_Register";

    public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Nickname);
}