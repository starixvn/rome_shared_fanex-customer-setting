using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class AgentPTOnMemberSettingItem : PTItem
{
    [JsonProperty(PropertyName = "AgentPT")]
    public decimal A1 { get; set; }

    [JsonIgnore]
    public decimal A2 { get; set; }

    [JsonIgnore]
    public decimal N2 { get; set; }

    public override bool Is3DecimalPTValue()
        => A1 * 10 % 10 != 0;
}