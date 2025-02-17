using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Rules;

public class CheckAllLimitedRulesRequest
{
    public int CustId { get; set; }

    public byte CustLevel { get; set; }

    public string ActorUsername { get; set; }

    public string Language { get; set; }

    public bool IsDirectMember { get; set; }

    public bool IsValid()
        => CustId > 0
        && CustLevel > UserLevel.Member.Value
        && CustLevel <= UserLevel.Super.Value;
}