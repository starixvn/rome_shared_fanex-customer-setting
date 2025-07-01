using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Data;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class RemoveMemberBankInfoCommand : NonQueryCommand
{
    public RemoveMemberBankInfoCommand(RemoveMemberBankInfoRequest request, int siteDBGroupId)
    {
        ActorId = request.ActorId;
        CustId = request.CustId;
        BankAccountMemberID = request.BankAccountMemberID;
        IP = request.IP;
        Language = request.Language;
        SubAccID = request.SubAccID;
        SiteDBGroupId = siteDBGroupId;
    }

    [SpParam(SqlParamName = "userID")]
    public int ActorId { get; }

    public int CustId { get; }

    public int BankAccountMemberID { get; }

    public string IP { get; }

    public string Language { get; }

    public int SubAccID { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }


    [SpParam(Ignored = true)]
    public int SiteDBGroupId { get; }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerDelBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}