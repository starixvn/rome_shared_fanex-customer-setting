using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetSuggestUsernameCommand : NonQueryCommand
{

    public GetSuggestUsernameCommand(string actorName, int custId, byte roleId)
    {
        ActorName = actorName;
        CustId = custId;
        RoleId = roleId;
    }

    [SpParam(SqlParamName = "Prefix")]
    public string ActorName { get; }

    public int CustId { get; }

    public byte RoleId { get; }

    public string SeqName { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelSuggestUsername";

    public override bool IsValid() => CustId > 0 && !string.IsNullOrEmpty(ActorName);
}