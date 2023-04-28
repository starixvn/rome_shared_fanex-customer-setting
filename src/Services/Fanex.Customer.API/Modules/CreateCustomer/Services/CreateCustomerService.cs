using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Shared.Constants;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    CustomerSetting Query(int custId);

}

public class CreateCustomerService : ICreateCustomerService
{
    public CustomerSetting Query(int custId)
    {
        return FakeData();
    }

    private static CustomerSetting FakeData()
    {
        var result = new CustomerSetting()
        {
            GeneralInfo = FakeGeneralInfo(CustomerLevel.Master),
            ProductGroups = FakeProductGroups()
        };

        return result;
    }

    private static GeneralInformation FakeGeneralInfo(Enum level)
    {
        return new GeneralInformation
        {
            UserName = "SuperTest1",
            AllowAutoPT = false,
            MaxCredit = 0,
            MemberMaxCredit = 0,
            NewUserName = level.Equals(CustomerLevel.Member) ? "001" : "01",
            TransferOption = "0101010",
            IsInTransferList = false
        };
    }

    private static List<ProductGroup> FakeProductGroups()
    {
        return new List<ProductGroup>
        {
          FakeSportsbookGroup()
        };
    }

    private static ProductGroup FakeSportsbookGroup()
    {
        return new ProductGroup
        {
            ProductGroupId = 1,
            DisplayOrder = 0,
            ProductGroupName = "Sportsbook",
            GroupTypes = new List<GroupType>{
                FakeGroupType150(),
                FakeGroupType151(),
            }
        };
    }

    private static GroupType FakeGroupType150()
    {
        return new GroupType
        {
            GroupTypeId = 150,
            GroupTypeName = "Commission",
            DisplayType = 5,
            GroupItems = new List<GroupItemDefault>{
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 1,
                            BettypeId = 1,
                            DisplayName="GroupA",
                            Discount = 0
                        },
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 1,
                            BettypeId = 3,
                            DisplayName="GroupB",
                            Discount = 0
                        },
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 1,
                            BettypeId = 7,
                            DisplayName="GroupC",
                            Discount = 0
                        },
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 1,
                            BettypeId = 8,
                            DisplayName="GroupD",
                            Discount = 0
                        },
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 1,
                            BettypeId = 4,
                            DisplayName="1x2 Comm",
                            Discount = 0
                        },
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 1,
                            BettypeId = 5,
                            DisplayName="Others Comm",
                            Discount = 0
                        },
                        new ViewModels.GroupType150.CommissionGroup{
                            SporttypeId = 2,
                            BettypeId = 1,
                            DisplayName="Myanmar Odds Comm",
                            Discount = 0
                        }
                    }
        };
    }

    private static GroupType FakeGroupType151()
    {
        return new GroupType
        {
            GroupTypeId = 151,
            GroupTypeName = "Bet Setting",
            DisplayType = 1,
            GroupItems = new List<GroupItemDefault>
            {
                new ViewModels.GroupType151.BetSettingItem(1, 1, "Soccer")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(1, 1, "Soccer")
                },
                new ViewModels.GroupType151.BetSettingItem(9901, 1, "Saba Soccer")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(9901, 1, "Saba Soccer")
                },
                new ViewModels.GroupType151.BetSettingItem(1, 9, "Soccer Parlay")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(1, 9, "Soccer Parlay")
                },
                new ViewModels.GroupType151.BetSettingItem(2, 1, "Basketball")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(2, 1, "Basketball")
                },
                new ViewModels.GroupType151.BetSettingItem(9902, 1, "Saba Basketball")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(9902, 1, "Saba Basketball")
                },
                new ViewModels.GroupType151.BetSettingItem(3, 1, "Football")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(3, 1, "Football")
                },
                new ViewModels.GroupType151.BetSettingItem(5, 1, "Tennis")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(5, 1, "Tennis")
                },
                new ViewModels.GroupType151.BetSettingItem(5, 1, "Tennis")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(5, 1, "Tennis")
                },
                new ViewModels.GroupType151.BetSettingItem(8, 1, "Baseball")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(8, 1, "Baseball")
                },
                new ViewModels.GroupType151.BetSettingItem(10, 1, "Goft")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(10, 1, "Golf")
                },
                new ViewModels.GroupType151.BetSettingItem(11, 1, "Moto Sports")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(11, 1, "Moto Sports")
                },
                new ViewModels.GroupType151.BetSettingItem(99, 1, "Other Sports")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(99, 1, "Other Sports")
                },
                new ViewModels.GroupType151.BetSettingItem(99, 9, "Mix Sports Parlay")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(99, 9, "Mix Sports Parlay")
                },
                new ViewModels.GroupType151.BetSettingItem(43, 1, "E-Sports")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(43, 1, "E-Sports")
                },
                new ViewModels.GroupType151.BetSettingItem(44, 3, "Muay Thai")
                {
                    Upline = new ViewModels.GroupType151.BetSettingItem(44, 3, "Muay Thai")
                }
            }
        };
    }
}