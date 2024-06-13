using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153.Constants;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class AgentPTSettingItem : PTItem
{
    [JsonProperty("MasterPT")]
    public decimal M2 { get; set; }

    [JsonProperty("AutoMasterPT")]
    public decimal UM2 { get; set; }

    [JsonProperty("IsAutoMasterPT")]
    public bool IM2 { get; set; }

    [JsonProperty("AgentPT")]
    public decimal A2 { get; set; }

    [JsonProperty("MinPT")]
    public decimal N2 { get; set; }

    [JsonIgnore]
    public decimal M3 { get; set; }

    [JsonIgnore]
    public decimal N3 { get; set; }

    [JsonIgnore]
    public decimal A4 { get; set; }

    [JsonProperty("IsUpdAllMems")]
    public bool IsUpdateMasterPTOnMembers { get; set; }

    [JsonProperty("UpdAllMems_Value")]
    public decimal MasterPTOnMembers { get; set; }

    public bool ShouldSerializeM2()
    {
        if (IM2)
        {
            M2 = 0;
        }
        else
        {
            UM2 = 0;
        }

        return true;
    }

    public override Result Validate(string language = "en-US")
    {
        if (N2 > A2)
        {
            return Result.CreateErrorResult(StaticResources.Get("AgentMinPTCannotBeGreaterThanAgentMaxPT", language), ErrorCodes.AgentMinPTIsGreaterThanAgentMaxPT);
        }

        return Result.CreateSuccessfulResult();
    }

    public override bool Is3DecimalPTValue()
        => (M2 * 10) % 10 != 0
            || (UM2 * 10) % 10 != 0
            || (A2 * 10) % 10 != 0
            || (N2 * 10) % 10 != 0
            || (MasterPTOnMembers * 10) % 10 != 0;
}
