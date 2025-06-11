using Fanex.Data.Repository;
using System;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService
{
    public class AgeCashServiceBillingDailyFeeInsCommand : NonQueryCommand
    {
        public int CustId { get; }
        public DateTime TrialEndTime { get; }
        public string IP { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string LogData { get; set; }

        public AgeCashServiceBillingDailyFeeInsCommand(int custId, DateTime trialEndTime, string ip)
        {
            CustId = custId;
            TrialEndTime = trialEndTime;
            IP = ip;
        }

        public override string GetSettingKey() => "Age_CashServiceBilling_DailyFee_Ins";
        public override bool IsValid() => true;
    }
}
