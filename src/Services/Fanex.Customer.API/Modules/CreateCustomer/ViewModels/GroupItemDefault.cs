using System.Text.Json.Serialization;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

[JsonDerivedType(typeof(CommissionGroup), typeDiscriminator: 1)]
[JsonDerivedType(typeof(BetSettingItem), typeDiscriminator: 2)]
[JsonDerivedType(typeof(ParlayMaxPayoutSettingItem), typeDiscriminator: 3)]
[JsonDerivedType(typeof(SuperPTSettingItem), typeDiscriminator: 4)]
[JsonDerivedType(typeof(MasterPTSettingItem), typeDiscriminator: 5)]
[JsonDerivedType(typeof(AgentPTSettingItem), typeDiscriminator: 6)]
[JsonDerivedType(typeof(GroupType154.BetSettingItem), typeDiscriminator: 7)]
[JsonDerivedType(typeof(CommissionSettingItem), typeDiscriminator: 8)]
[JsonDerivedType(typeof(MemberPTItem), typeDiscriminator: 9)]
[JsonDerivedType(typeof(GroupType157.BetSettingItem), typeDiscriminator: 10)]
public class GroupItemDefault
{
    private const string SportTypeBetTypeXmlFormat = "SportType=\"{0}\" BetType=\"{1}\"";

    public int SportTypeId { get; set; }

    public string SportTypeName { get; set; }

    public int BetTypeId { get; set; }

    public string BetTypeName { get; set; }

    public string DisplayName { get; set; }

    public int? SubGroupId { get; set; }

    public string SubGroupName { get; set; }

    public bool IsLive { get; set; }

    public virtual string ToXml()
        => string.Format(SportTypeBetTypeXmlFormat, SportTypeId, BetTypeId);

    public virtual bool Validate()
        => true;

    public virtual bool Is3DecimalPTValue()
        => false;
}