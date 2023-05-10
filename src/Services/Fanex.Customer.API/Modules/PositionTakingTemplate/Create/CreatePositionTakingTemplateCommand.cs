using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class CreatePositionTakingTemplateCommand : INonQueryCommand
{
    public CreatePositionTakingTemplateCommand(
        int custId,
        string templateName,
        string settings,
        bool isDefault,
        byte creationStatus,
        string language,
        string ip)
    {
        CustId = custId;
        TemplateName = templateName;
        Settings = settings;
        IsDefault = isDefault;
        CreationStatus = creationStatus;
        Language = language;
        IP = ip;
    }

    public int CustId { get; }

    public string TemplateName { get; }

    public string Settings { get; }

    public bool IsDefault { get; }

    public byte CreationStatus { get; }

    public string Language { get; }

    public string IP { get; }

    public int ErrorCode { get; set; }

    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustSetting_CustPTTemplateIns";

    public bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(TemplateName) && !string.IsNullOrWhiteSpace(Settings);
}