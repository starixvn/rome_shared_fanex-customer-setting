namespace Fanex.Customer.API.Modules.Customer.Models;

public record Downline(bool IsActive, int CustId, bool IsSubAcc, int RoleId);