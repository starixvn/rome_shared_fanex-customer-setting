using Fanex.Data.Repository;
using System;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService
{
    public class AgeCashServiceCustServiceTypeInitializeCommand : NonQueryCommand
    {
        public int CustID { get; }
        public int ServiceType { get; }
        public string Site { get; }
        public string IP { get; }
        public string Language { get; }

        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public DateTime? TrialEndTime { get; set; }
        // Add properties for dataLog output if needed

        public AgeCashServiceCustServiceTypeInitializeCommand(int custId, int serviceType, string site, string ip, string language)
        {
            CustID = custId;
            ServiceType = serviceType;
            Site = site;
            IP = ip;
            Language = language;
        }

        public override string GetSettingKey() => "Age_CashService_CustServiceTypeInitialize";
        public override bool IsValid() => true;
    }
}
