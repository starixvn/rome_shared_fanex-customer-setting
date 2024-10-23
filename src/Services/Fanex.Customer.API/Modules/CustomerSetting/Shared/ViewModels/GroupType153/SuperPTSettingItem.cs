using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153.Constants;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class SuperPTSettingItem : PTItem
{
    [JsonProperty(PropertyName = "SuperPT")]
    public decimal S4 { get; set; }

    [JsonProperty(PropertyName = "MasterPT")]
    public decimal M4 { get; set; }

    [JsonProperty(PropertyName = "AgentPT")]
    public decimal A4 { get; set; }

    [JsonProperty(PropertyName = "MinPT")]
    public decimal N4 { get; set; }

    [JsonIgnore]
    public decimal DefaultMaxPT { get; set; }

    public override Result Validate(string language = "en-US")
    {
        if (N4 > S4)
        {
            return Result.CreateErrorResult(StaticResources.Get("SuperMinPTCannotBeGreaterThanSuperMaxPT", language), ErrorCodes.SuperMinPTIsGreaterThanSuperMaxPTErrorCode);
        }

        return Result.CreateSuccessfulResult();
    }

    public override bool Is3DecimalPTValue()
        => S4 * 10 % 10 != 0
            || M4 * 10 % 10 != 0
            || N4 * 10 % 10 != 0;
}
