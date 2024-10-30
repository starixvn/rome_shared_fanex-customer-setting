using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command;

public class RegisterSubAccountCriteria(int custId, string username, string nickname, string language, int custDBGroupID)
    : NonQueryCommand
{
    public int CustId => custId;

    public string Username => username;

    public string Nickname => nickname;

    public string Language => language;

    public int SiteDBGroupID => custDBGroupID;

    public int SubAccId { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_CustSubIns_Register";

    public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Username);
}