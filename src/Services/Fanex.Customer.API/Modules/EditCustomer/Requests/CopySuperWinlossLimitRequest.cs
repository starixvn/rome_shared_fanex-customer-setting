namespace Fanex.Customer.API.Modules.EditCustomer.Requests;

public class CopySuperWinlossLimitRequest
{
    public int SourceSuperId { get; set; }

    public string SourceUserName { get; set; }

    public string CopiedSuperIds { get; set; }

    public string CopiedUserNames { get; set; }

    public int ActorId { get; set; }

    public byte SystemId { get; set; }

    public string Language { get; set; }

    public string IP { get; set; }

    public IEnumerable<int> SportTypeIds { get; set; }
}