using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class CheckCashMemberCommand : NonQueryCommand
{
    public CheckCashMemberCommand(int uplineCustId, string userName)
    {
        UplineCustId = uplineCustId;
        UserName = userName;
    }

    public int UplineCustId { get; }

    public string UserName { get; }

    public override string GetSettingKey() => "Age_CustomerSearch_CashMember";

    public override bool IsValid() => UplineCustId > 0 && !string.IsNullOrWhiteSpace(UserName);
}