using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class RegisterSubAccNicknameCommand(int custId, int subAccId, string nickname, string language, int siteDBGroupID) : NonQueryCommand
{
    public int CustId => custId;

    public int SubAccId => subAccId;

    public string Nickname => nickname;

    public string Language => language;

    public int SiteDBGroupID => siteDBGroupID;

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_CustSubIns_Register";

    public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Nickname);
}