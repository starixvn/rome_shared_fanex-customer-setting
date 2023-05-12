using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTakingTemplate;

public class UpdatePositionTakingTemplateCommand : INonQueryCommand
{
    public UpdatePositionTakingTemplateCommand(
        int custId,
        int templateId,
        string settings,
        bool isDefault,
        byte creationStatus,
        string language,
        string ip)
    {
        CustId = custId;
        TemplateId = templateId;
        Settings = settings;
        IsDefault = isDefault;
        CreationStatus = creationStatus;
        Language = language;
        IP = ip;
    }

    public int CustId { get; }

    public int TemplateId { get; }

    public string Settings { get; }

    public bool IsDefault { get; }

    public byte CreationStatus { get; }

    public string Language { get; }

    public string IP { get; }

    public int ErrCode { get; set; }

    public int ErrType { get; set; }

    public string ErrMsg { get; set; }

    public string GetSettingKey() => "Acc_LiteMode_CustSetting_CustPTTemplateUpd";

    public bool IsValid() => CustId > 0 && TemplateId > 0 && !string.IsNullOrWhiteSpace(Settings);
}