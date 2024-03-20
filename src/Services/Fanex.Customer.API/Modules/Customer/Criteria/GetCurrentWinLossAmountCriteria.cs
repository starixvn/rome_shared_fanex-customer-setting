using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetCurrentWinLossAmountCriteria : CriteriaBase
{
    public GetCurrentWinLossAmountCriteria(int custId, int sportType, int betType, short settingStatus, string siteDBName)
    {
        CustId = custId;
        SportType = sportType;
        BetType = betType;
        SettingStatus = settingStatus;
        SiteDBName = siteDBName;
    }

    public int CustId { get; }

    public int SportType { get; }

    public int BetType { get; }

    public short SettingStatus { get; }

    [SpParam(Ignored = true)]
    public string SiteDBName { get; }

    public override string GetSettingKey() => $"{SiteDBName}_Acc_CustSetting_WinLossPercentSelect";

    public override bool IsValid() => CustId > 0;
}
