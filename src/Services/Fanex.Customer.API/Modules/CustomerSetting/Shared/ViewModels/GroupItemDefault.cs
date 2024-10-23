using System.Text.Json.Serialization;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType158;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType160;
using Fanex.Customer.API.Shared.Models;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

[JsonDerivedType(typeof(SuperCommissionGroup), typeDiscriminator: 1)]
[JsonDerivedType(typeof(MasterCommissionGroup), typeDiscriminator: 2)]
[JsonDerivedType(typeof(AgentCommissionGroup), typeDiscriminator: 3)]
[JsonDerivedType(typeof(MemberCommissionGroup), typeDiscriminator: 4)]
//[JsonDerivedType(typeof(CommissionGroup), typeDiscriminator: 5)]
[JsonDerivedType(typeof(BetSettingItem), typeDiscriminator: 6)]
[JsonDerivedType(typeof(ParlayMaxPayoutSettingItem), typeDiscriminator: 7)]
[JsonDerivedType(typeof(SuperPTSettingItem), typeDiscriminator: 8)]
[JsonDerivedType(typeof(MasterPTSettingItem), typeDiscriminator: 9)]
[JsonDerivedType(typeof(AgentPTSettingItem), typeDiscriminator: 10)]
[JsonDerivedType(typeof(AgentPTOnMemberSettingItem), typeDiscriminator: 11)]
[JsonDerivedType(typeof(MasterPTOnMemberSettingItem), typeDiscriminator: 12)]
[JsonDerivedType(typeof(SuperPTOnMemberSettingItem), typeDiscriminator: 13)]
[JsonDerivedType(typeof(GroupType154.BetSettingItem), typeDiscriminator: 14)]
[JsonDerivedType(typeof(WinLossLimitItem), typeDiscriminator: 15)]
[JsonDerivedType(typeof(CommissionSettingItem), typeDiscriminator: 16)]
[JsonDerivedType(typeof(GroupType157.BetSettingItem), typeDiscriminator: 17)]
[JsonDerivedType(typeof(StakeLevelItem), typeDiscriminator: 18)]
[JsonDerivedType(typeof(ProductStatusItem), typeDiscriminator: 19)]
public class GroupItemDefault
{
    public int ProductId { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string ProductName { get; set; }

    [JsonProperty(PropertyName = "SportType")]
    public int SportTypeId { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string SportTypeName { get; set; }

    [JsonProperty(PropertyName = "BetType")]
    public int BetTypeId { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string BetTypeName { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string DisplayName { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public int? SubGroupId { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string SubGroupName { get; set; }

    public bool IsLive { get; set; }

    public virtual bool ShouldSerializeIsLive() => true;

    public virtual void MapUpline() { }

    public virtual Result Validate(string language = "en-US")
        => Result.CreateSuccessfulResult();

    public virtual bool Is3DecimalPTValue()
        => false;
}