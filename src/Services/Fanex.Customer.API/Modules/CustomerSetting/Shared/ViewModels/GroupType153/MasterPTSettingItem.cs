using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153.Constants;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;

public class MasterPTSettingItem : PTItem
{
    [JsonProperty("SuperPT")]
    public decimal S3 { get; set; }

    [JsonProperty("AutoSuperPT")]
    public decimal US3 { get; set; }

    [JsonProperty("IsAutoSuperPT")]
    public bool IS3 { get; set; }

    [JsonProperty("MasterPT")]
    public decimal M3 { get; set; }

    [JsonProperty("MinPT")]
    public decimal N3 { get; set; }

    [JsonIgnore]
    public decimal S4 { get; set; }

    [JsonIgnore]
    public decimal M4 { get; set; }

    [JsonIgnore]
    public decimal N4 { get; set; }

    [JsonProperty("IsUpdAllMems")]
    public bool IsUpdateSuperPTOnMembers { get; set; }

    [JsonProperty("UpdAllMems_Value")]
    public decimal SuperPTOnMembers { get; set; }

    public bool ShouldSerializeS3()
    {
        if (IS3)
        {
            S3 = 0;
        }
        else
        {
            US3 = 0;
        }

        return true;
    }

    public override Result Validate(string language = "en-US")
    {
        if (N3 > M3)
        {
            return Result.CreateErrorResult(StaticResources.Get("MasterMinPTCannotBeGreaterThanMasterMaxPT", language), ErrorCodes.MasterMinPTIsGreaterThanMasterMaxPT);
        }

        return Result.CreateSuccessfulResult();
    }

    public override bool Is3DecimalPTValue()
        => S3 * 10 % 10 != 0
            || US3 * 10 % 10 != 0
            || M3 * 10 % 10 != 0
            || N3 * 10 % 10 != 0
            || SuperPTOnMembers * 10 % 10 != 0;
}
