using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class RegisterSubAccNicknameCommand(int custId, string nickname, int subAccId, string language, int siteDBGroupID) : NonQueryCommand
{
    public int CustId => custId;

    public int SiteDBGroupID => siteDBGroupID;

    public string Language => language;

    public string Nickname => nickname;

    public int SubAccId { get; set; } = subAccId;

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_CustSubIns_Register";

    public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Nickname);
}