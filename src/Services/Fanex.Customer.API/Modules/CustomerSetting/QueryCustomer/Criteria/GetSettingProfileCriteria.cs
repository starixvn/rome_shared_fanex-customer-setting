using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.QueryCustomer.Criteria;

public class GetSettingProfileCriteria : CriteriaBase
{
    public GetSettingProfileCriteria(byte settingStatus, string site, int currency, byte systemId, byte mode, byte serviceType)
    {
        SettingStatus = settingStatus;
        Site = site;
        Currency = currency;
        SystemId = systemId;
        Mode = mode;
        ServiceType = serviceType;
    }

    public byte SettingStatus { get; }

    public string Site { get; }

    public int Currency { get; }

    public int SystemId { get; }

    public byte Mode { get; }

    public byte ServiceType  { get; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_ProductProfileSelect";

    public override bool IsValid() => true;
}
