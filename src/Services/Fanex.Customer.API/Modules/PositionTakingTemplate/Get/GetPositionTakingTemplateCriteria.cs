using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class GetPositionTakingTemplateCriteria : CriteriaBase
{
    public GetPositionTakingTemplateCriteria(int custId, byte settingStatus)
    {
        CustId = custId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_CustPTTemplateSel";

    public override bool IsValid() => CustId > 0;
}