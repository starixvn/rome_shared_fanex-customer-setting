using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class UpdateMemberBankInfoCommand(UpdateMemberBankInfoRequest request, int siteDBGroupId) : AddMemberBankInfoCommand(request, siteDBGroupId)
{
    public int MemberBankAccountId { get; } = request.MemberBankAccountId;

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerUpdBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}