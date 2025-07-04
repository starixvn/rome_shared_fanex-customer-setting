using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class RemoveMemberBankInfoCommand(RemoveMemberBankInfoRequest request, int siteDBGroupId) : BaseMemberBankInfoCommand(
    request.ActorId,
    request.CustId,
    request.IP,
    request.Language,
    request.SubAccId,
    siteDBGroupId)
{
    [SpParam(SqlParamName = "BankAccountMemberID")]
    public int MemberBankAccountId { get; } = request.MemberBankAccountId;

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerDelBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}