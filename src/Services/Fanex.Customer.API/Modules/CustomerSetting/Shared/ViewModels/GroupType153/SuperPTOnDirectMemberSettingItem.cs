using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class SuperPTOnDirectMemberSettingItem : PTItem
{
    [JsonIgnore]
    public decimal S4 { get; set; }

    [JsonIgnore]
    public decimal N4 { get; set; }

    [JsonProperty("SuperPT")]
    public decimal S1 { get; set; }

    public override bool Is3DecimalPTValue()
    => S4 * 10 % 10 != 0
        || N4 * 10 % 10 != 0
        || S1 * 10 % 10 != 0;
}
