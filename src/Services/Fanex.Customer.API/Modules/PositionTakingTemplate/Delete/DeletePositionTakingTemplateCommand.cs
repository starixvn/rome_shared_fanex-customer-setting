using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class DeletePositionTakingTemplateCommand : INonQueryCommand
{
    public DeletePositionTakingTemplateCommand(int custId, int templateId, string language, string ip)
    {
        CustId = custId;
        TemplateId = templateId;
        Language = language;
        IP = ip;
    }

    public int CustId { get; }

    public int TemplateId { get; }

    public string Language { get; }

    public string IP { get; }

    public int ErrorCode { get; set; }

    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustSetting_CustPTTemplateDel";

    public bool IsValid() => CustId > 0 && TemplateId > 0;
}