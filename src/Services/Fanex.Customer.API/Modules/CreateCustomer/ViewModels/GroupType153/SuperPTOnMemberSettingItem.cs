using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class SuperPTOnMemberSettingItem : PTItem
{
    [JsonIgnore]
    public decimal A1 { get; set; }

    [JsonIgnore]
    public decimal M1 { get; set; }

    [JsonProperty("SuperPT")]
    public decimal S1 { get; set; }

    [JsonProperty("AutoSuperPT")]
    public decimal US1 { get; set; }

    [JsonProperty("IsAutoSuperPT")]
    public bool IS1 { get; set; }

    [JsonIgnore]
    public bool IS3 { get; set; }

    [JsonIgnore]
    public decimal S4 { get; set; }

    [JsonIgnore]
    public decimal N4 { get; set; }

    [JsonIgnore]
    public decimal M3 { get; set; }

    public bool ShouldSerializeS1()
    {
        if (IS1)
        {
            S1 = 0;
        }
        else
        {
            US1 = 0;
        }

        return true;
    }

    public override bool Is3DecimalPTValue()
    => A1 * 10 % 10 != 0
        || M1 * 10 % 10 != 0
        || S1 * 10 % 10 != 0
        || US1 * 10 % 10 != 0
        || S4 * 10 % 10 != 0
        || M3 * 10 % 10 != 0;
}
