using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class UpdateMemberBankInfoCommand(UpdateMemberBankInfoRequest request, int siteDBGroupId) : BaseMemberBankInfoCommand(
    request.ActorId,
    request.CustId,
    request.MemberBankId,
    request.MemberBankAccountNumber,
    request.AgentBankAccountNumber,
    request.AgentBankId,
    request.IP,
    request.Language,
    request.SubAccId,
    siteDBGroupId)
{
    public int MemberBankAccountId { get; } = request.MemberBankAccountId;

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerUpdBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}