namespace Fanex.Customer.API.Shared.Models;

public record SiteDBGroup(int SiteDbGroupId, int SiteId, string Site, bool IsDeposit, string LinkedServerName);