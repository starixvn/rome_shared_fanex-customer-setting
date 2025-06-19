using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Criteria
{
    public class GetJackpotStatusTooltipCriteria : CriteriaBase
    {
        public int CustId { get; }
        public int SettingStatus { get; }

        public GetJackpotStatusTooltipCriteria(int custId, int settingStatus)
        {
            CustId = custId;
            SettingStatus = settingStatus;
        }

        public override bool IsValid() => CustId > 0;
        public override string GetSettingKey() => "Acc_CustSetting_JackpotStatusTooltip";
    }
}
