using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetSuggestUsernameCommand : NonQueryCommand
{
    public GetSuggestUsernameCommand(string actorName, byte roleId)
    {
        ActorName = actorName;
        RoleId = roleId;
    }

    [SpParam(SqlParamName = "Prefix")]
    public string ActorName { get; }

    public byte RoleId { get; }

    public string SeqName { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelSuggestUsernameNew";

    public override bool IsValid() => !string.IsNullOrEmpty(ActorName);
}