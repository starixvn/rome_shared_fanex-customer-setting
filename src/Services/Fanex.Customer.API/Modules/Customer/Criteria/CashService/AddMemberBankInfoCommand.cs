using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class AddMemberBankInfoCommand : BaseMemberBankInfoCommand
{
    public AddMemberBankInfoCommand(AddMemberBankInfoRequest request, int siteDBGroupId) : base(
        request.ActorId,
        request.CustId,
        request.MemberBankId,
        request.MemberBankAccountNumber,
        request.AgentBankAccountNumber,
        request.AgentBankId,
        request.IP,
        request.Language,
        request.SubAccID,
        siteDBGroupId)
    {
    }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerInsBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}