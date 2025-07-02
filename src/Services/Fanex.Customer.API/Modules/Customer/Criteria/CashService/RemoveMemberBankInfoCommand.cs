using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Data;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class RemoveMemberBankInfoCommand : BaseMemberBankInfoCommand
{
    public RemoveMemberBankInfoCommand(RemoveMemberBankInfoRequest request, int siteDBGroupId) : base(
        request.ActorId,
        request.CustId,
        request.IP,
        request.Language,
        request.SubAccID,
        siteDBGroupId)
    {
        MemberBankAccountId = request.MemberBankAccountId;
    }

    public int MemberBankAccountId { get; }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerDelBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}