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
[JsonDerivedType(typeof(SuperDirectMemberCommissionGroup), typeDiscriminator: 5)]
[JsonDerivedType(typeof(MasterDirectMemberCommissionGroup), typeDiscriminator: 6)]
//[JsonDerivedType(typeof(CommissionGroup), typeDiscriminator: 7)]
[JsonDerivedType(typeof(BetSettingItem), typeDiscriminator: 8)]
[JsonDerivedType(typeof(ParlayMaxPayoutSettingItem), typeDiscriminator: 9)]
[JsonDerivedType(typeof(SuperPTSettingItem), typeDiscriminator: 10)]
[JsonDerivedType(typeof(MasterPTSettingItem), typeDiscriminator: 11)]
[JsonDerivedType(typeof(AgentPTSettingItem), typeDiscriminator: 12)]
[JsonDerivedType(typeof(AgentPTOnMemberSettingItem), typeDiscriminator: 13)]
[JsonDerivedType(typeof(MasterPTOnMemberSettingItem), typeDiscriminator: 14)]
[JsonDerivedType(typeof(MasterPTOnDirectMemberSettingItem), typeDiscriminator: 15)]
[JsonDerivedType(typeof(SuperPTOnMemberSettingItem), typeDiscriminator: 16)]
[JsonDerivedType(typeof(SuperPTOnDirectMemberSettingItem), typeDiscriminator: 17)]
[JsonDerivedType(typeof(GroupType154.BetSettingItem), typeDiscriminator: 18)]
[JsonDerivedType(typeof(WinLossLimitItem), typeDiscriminator: 19)]
//[JsonDerivedType(typeof(CommissionSettingItem), typeDiscriminator: 20)]
[JsonDerivedType(typeof(GroupType157.BetSettingItem), typeDiscriminator: 21)]
[JsonDerivedType(typeof(StakeLevelItem), typeDiscriminator: 22)]
[JsonDerivedType(typeof(ProductStatusItem), typeDiscriminator: 23)]
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
        => Result.CreateSuccess();

    public virtual bool Is3DecimalPTValue()
        => false;
}