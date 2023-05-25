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

[JsonDerivedType(typeof(CommissionGroup))]
[JsonDerivedType(typeof(GroupType151.BetSettingItem))]
[JsonDerivedType(typeof(ParlayMaxPayoutSettingItem))]
[JsonDerivedType(typeof(SuperPTSettingItem))]
[JsonDerivedType(typeof(GroupType154.BetSettingItem))]
[JsonDerivedType(typeof(WinlossLimitSettingItem))]
[JsonDerivedType(typeof(GroupType156.CommissionSettingItem))]
[JsonDerivedType(typeof(LossLimitSettingItem))]
[JsonDerivedType(typeof(GroupType158.CommissionSettingItem))]
[JsonDerivedType(typeof(GroupType159.BetsettingItem))]
public class GroupItemDefault
{
    public GroupItemDefault() : this(0, 0, null) { }

    public GroupItemDefault(int sporttypeid, int bettypeid, string DisplayName)
    {
        this.SporttypeId = sporttypeid;
        this.BettypeId = bettypeid;
        this.DisplayName = DisplayName;
    }

    public int SporttypeId { get; set; }

    public string SporttypeName { get; set; }

    public int BettypeId { get; set; }

    public string BettypeName { get; set; }

    public string DisplayName { get; set; }

    public int? SubGroupId { get; set; }

    public string SubGroupName { get; set; }

    internal string ToXml()
    {
        throw new NotImplementedException();
    }
}