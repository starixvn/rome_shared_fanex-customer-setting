using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class AddMemberBankInfoCommand(AddMemberBankInfoRequest request, int siteDBGroupId) : BaseMemberBankInfoCommand(
    request.ActorId,
    request.CustId,
    request.IP,
    request.Language,
    request.SubAccId,
    siteDBGroupId)
{
    public int MemberBankId { get; } = request.MemberBankId;

    public string MemberBankAccountNumber { get; } = request.MemberBankAccountNumber;

    public string AgentBankAccountNumber { get; } = request.AgentBankAccountNumber;

    public int AgentBankId { get; } = request.AgentBankId;
    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerInsBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}