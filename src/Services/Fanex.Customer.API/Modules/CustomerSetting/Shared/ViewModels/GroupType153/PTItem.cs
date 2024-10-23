using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class PTItem : GroupItemDefault
{
    [JsonIgnore]
    public decimal Step { get; set; }

    [JsonIgnore]
    public bool IsUpdateMinPtOnly { get; set; }
}
