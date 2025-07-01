using Fanex.Data.Repository;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class AddMemberBankInfoCommand : NonQueryCommand
{
    public AddMemberBankInfoCommand(AddMemberBankInfoRequest request, int siteDBGroupId)
    {
        CustId = request.CustId;
        ActorId = request.ActorId;
        MemberBankId = request.MemberBankId;
        MemberBankAccountNumber = request.MemberBankAccountNumber;
        AgentBankAccountNumber = request.AgentBankAccountNumber;
        IP = request.IP;
        Language = request.Language;
        SubAccID = request.SubAccID;
        SiteDBGroupId = siteDBGroupId;
    }

    [SpParam(SqlParamName = "userID")]
    public int ActorId { get; }

    public int CustId { get; }

    [SpParam(SqlParamName = "BankId")]
    public int MemberBankId { get; }

    [SpParam(SqlParamName = "AccountNumber")]
    public int MemberBankAccountNumber { get; }


    [SpParam(SqlParamName = "AccountNumberAgent")]
    public int AgentBankAccountNumber { get; }


    public string IP { get; }

    public string Language { get; }

    public int SubAccID { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }


    [SpParam(Ignored = true)]
    public int SiteDBGroupId { get; }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("Acc_CustomerInsBankAccount_CashMember", SiteDBGroupId);

    public override bool IsValid() => CustId > 0;
}