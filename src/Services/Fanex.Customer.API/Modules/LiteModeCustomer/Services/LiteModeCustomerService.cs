using Fanex.Customer.API.Modules.LiteModeCustomer.Constants;
using Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.LiteModeCustomer.Services;

public interface ILiteModeCustomerService
{
    LiteModeCustomerSetting Query(int custId);

}

public class LiteModeCustomerService : ILiteModeCustomerService
{
    public LiteModeCustomerSetting Query(int custId)
    {
        return FakeData();
    }

    private LiteModeCustomerSetting FakeData()
    {
        var result = new LiteModeCustomerSetting() {
            GeneralInfo= FakeGeneralInfo(CustomerLevel.Master)
         };

        return result;
    }

    private GeneralInfomation FakeGeneralInfo(Enum level)
    {
        return new GeneralInfomation
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
}