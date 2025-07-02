using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class UpdateMemberBankInfoCommand : BaseMemberBankInfoCommand
{
    public UpdateMemberBankInfoCommand(UpdateMemberBankInfoRequest request, int siteDBGroupId) : base(
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
        MemberBankAccountId = request.MemberBankAccountId;
    }

    public int MemberBankAccountId { get; }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerUpdBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}