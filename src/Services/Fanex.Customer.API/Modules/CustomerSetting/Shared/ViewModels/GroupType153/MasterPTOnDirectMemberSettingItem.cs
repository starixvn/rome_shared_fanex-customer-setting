using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class MasterPTOnDirectMemberSettingItem : PTItem
{
    [JsonIgnore]
    public decimal N3 { get; set; }

    [JsonIgnore]
    public decimal M3 { get; set; }

    [JsonProperty("MasterPT")]
    public decimal M1 { get; set; }

    public override bool Is3DecimalPTValue()
    => M1 * 10 % 10 != 0
        || M3 * 10 % 10 != 0
        || N3 * 10 % 10 != 0;
}
