using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class MasterPTOnMemberSettingItem : PTItem
{
    [JsonIgnore]
    public decimal A1 { get; set; }

    [JsonProperty(PropertyName = "MasterPT")]
    public decimal M1 { get; set; }

    [JsonProperty(PropertyName = "IsAutoMasterPT")]
    public bool IM1 { get; set; }

    [JsonProperty(PropertyName = "AutoMasterPT")]
    public decimal UM1 { get; set; }

    [JsonIgnore]
    public bool IM2 { get; set; }

    [JsonIgnore]
    public decimal M3 { get; set; }

    [JsonIgnore]
    public decimal N3 { get; set; }
    
    public bool ShouldSerializeM1()
    {
        if (IM1)
        {
            M1 = 0;
        }
        else
        {
            UM1 = 0;
        }

        return true;
    }

    public override bool Is3DecimalPTValue()
    => A1 * 10 % 10 != 0
        || M1 * 10 % 10 != 0
        || UM1 * 10 % 10 != 0
        || M3 * 10 % 10 != 0
        || N3 * 10 % 10 != 0;


}
