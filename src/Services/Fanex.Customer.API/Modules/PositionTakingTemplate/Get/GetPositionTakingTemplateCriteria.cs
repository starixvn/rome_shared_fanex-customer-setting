using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class GetPositionTakingTemplateCriteria : CriteriaBase
{
    public GetPositionTakingTemplateCriteria(int custId, byte creationStatus)
    {
        CustId = custId;
        CreationStatus = creationStatus;
    }

    public int CustId { get; }

    public byte CreationStatus { get; }

    public override string GetSettingKey() => "Acc_LiteMode_CustSetting_CustPTTemplateSel";

    public override bool IsValid() => CustId > 0;
}