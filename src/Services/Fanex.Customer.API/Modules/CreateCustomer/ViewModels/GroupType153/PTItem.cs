using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class PTItem : GroupItemDefault
{
    [JsonIgnore]
    public decimal Step { get; set; }

    [JsonIgnore]
    public bool IsUpdateMinPtOnly { get; set; }
}
