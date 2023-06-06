using System.Reflection;
using System.Text.Json.Serialization;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType154;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType156;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType158;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType159;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

[JsonDerivedType(typeof(CommissionGroup), typeDiscriminator: 1)]
[JsonDerivedType(typeof(GroupType151.BetSettingItem), typeDiscriminator: 2)]
[JsonDerivedType(typeof(ParlayMaxPayoutSettingItem), typeDiscriminator: 3)]
[JsonDerivedType(typeof(SuperPTSettingItem), typeDiscriminator: 4)]
[JsonDerivedType(typeof(GroupType154.BetSettingItem), typeDiscriminator: 5)]
[JsonDerivedType(typeof(WinlossLimitSettingItem), typeDiscriminator: 6)]
[JsonDerivedType(typeof(GroupType156.CommissionSettingItem), typeDiscriminator: 7)]
[JsonDerivedType(typeof(LossLimitSettingItem), typeDiscriminator: 8)]
[JsonDerivedType(typeof(GroupType158.CommissionSettingItem), typeDiscriminator: 9)]
[JsonDerivedType(typeof(GroupType159.BetsettingItem), typeDiscriminator: 10)]
public class GroupItemDefault
{
    private const string SportTypeBetTypeXmlFormat = "SportType=\"{0}\" BetType=\"{1}\"";

    public GroupItemDefault() : this(0, 0, null) { }

    public GroupItemDefault(int sporttypeid, int bettypeid, string DisplayName)
    {
        this.SportTypeId = sporttypeid;
        this.BetTypeId = bettypeid;
        this.DisplayName = DisplayName;
    }

    public int SportTypeId { get; set; }

    public string SportTypeName { get; set; }

    public int BetTypeId { get; set; }

    public string BetTypeName { get; set; }

    public string DisplayName { get; set; }

    public int? SubGroupId { get; set; }

    public string SubGroupName { get; set; }

    public virtual string ToXml()
        => string.Format(SportTypeBetTypeXmlFormat, SportTypeId, BetTypeId);

}