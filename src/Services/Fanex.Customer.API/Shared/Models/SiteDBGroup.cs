namespace Fanex.Customer.API.Shared.Models;

public record SiteDBGroup()
{
    public int SiteDbGroupId { get; init; }

    public int SiteId { get; init; }

    public string Site { get; init; }

    public bool IsDeposit { get; init; }

    public string LinkedServerName { get; init; }
}