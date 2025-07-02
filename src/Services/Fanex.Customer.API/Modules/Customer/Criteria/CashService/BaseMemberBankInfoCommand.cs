using Fanex.Data.Repository;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.Customer.Criteria.CashService;

public class BaseMemberBankInfoCommand : NonQueryCommand
{
    public BaseMemberBankInfoCommand(
        int actorId,
        int custId,
        int memberBankId,
        string memberBankAccountNumber,
        string agentBankAccountNumber,
        int agentBankId,
        string ip,
        string language,
        int subAccID,
        int siteDBGroupId)
    {
        ActorId = actorId;
        CustId = custId;
        MemberBankId = memberBankId;
        MemberBankAccountNumber = memberBankAccountNumber;
        AgentBankAccountNumber = agentBankAccountNumber;
        AgentBankId = agentBankId;
        IP = ip;
        Language = language;
        SubAccID = subAccID;
        SiteDBGroupId = siteDBGroupId;
    }
    public BaseMemberBankInfoCommand(
        int actorId,
        int custId,
        string ip,
        string language,
        int subAccID,
        int siteDBGroupId)
    {
        ActorId = actorId;
        CustId = custId;
        IP = ip;
        Language = language;
        SubAccID = subAccID;
        SiteDBGroupId = siteDBGroupId;
    }

    [SpParam(SqlParamName = "userID")]
    public int ActorId { get; }

    public int CustId { get; set; }

    public int MemberBankId { get; }

    public string MemberBankAccountNumber { get; }

    public string AgentBankAccountNumber { get; }

    public int AgentBankId { get; }

    public string IP { get; }

    public string Language { get; }

    public int SubAccID { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    [SpParam(Ignored = true)]
    public int SiteDBGroupId { get; }

    public override string GetSettingKey() => "";

    public override bool IsValid() => true;
}