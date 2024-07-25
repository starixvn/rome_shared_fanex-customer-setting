using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType160;

public class ProductStatusItem : GroupItemDefault
{
    [JsonIgnore]
    public int? ParentSportType { get; set; }

    [JsonIgnore]
    public int? ParentBetType { get; set; }

    public bool IsDisabled { get; set; }

    [JsonIgnore]
    public bool IsDisabledUpline { get; set; }

    public override bool ShouldSerializeIsLive() => false;
}
