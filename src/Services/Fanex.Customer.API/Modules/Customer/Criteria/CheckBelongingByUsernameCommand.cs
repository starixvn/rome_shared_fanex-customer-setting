
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class CheckBelongingByUsernameCommand : NonQueryCommand
{
    public CheckBelongingByUsernameCommand(int actorId, string username, int subAccountId)
    {
        ActorId = actorId;
        Username = username;
        SubAccountId = subAccountId;
    }

    [SpParam(SqlParamName = "uplineID")]
    public int ActorId { get; }

    public string Username { get; }

    public int SubAccountId { get; }

    public bool IsActive { get; set; }

    public int CustId { get; set; }

    public bool IsSubAcc { get; set; }

    public int RoleId { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelCheckBelonging_ByUsername";

    public override bool IsValid() => ActorId > 0 && !string.IsNullOrEmpty(Username);
}