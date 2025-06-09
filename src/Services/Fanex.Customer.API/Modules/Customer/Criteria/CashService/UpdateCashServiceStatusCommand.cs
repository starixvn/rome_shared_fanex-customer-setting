using Fanex.Data.Repository;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class UpdateCashServiceStatusCommand : NonQueryCommand
{
    public UpdateCashServiceStatusCommand(int custId, byte serviceType, int subAccId = 0, string ip = "", string language = "E", int siteDBGroupId = 0)
    {
        CustId = custId;
        ServiceType = serviceType;
        SubAccId = subAccId;
        IP = ip;
        Language = language;
        SiteDBGroupId = siteDBGroupId;
    }

    public int CustId { get; }
    public byte ServiceType { get; }
    public int SubAccId { get; }
    public string IP { get; }
    public string Language { get; }

    [SpParam(Ignored = true)]
    public int SiteDBGroupId { get; }

    public int ErrCode { get; set; }
    public string ErrMsg { get; set; }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Age_CashService_CustServiceTypeUpdate", SiteDBGroupId);

    public override bool IsValid() => true;
}
