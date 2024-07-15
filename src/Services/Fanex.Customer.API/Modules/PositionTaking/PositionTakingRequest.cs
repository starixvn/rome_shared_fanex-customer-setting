using Fanex.Customer.API.Modules.PositionTaking.Models;

namespace Fanex.Customer.API.Modules.PositionTaking;

public class PositionTakingRequest
{
    public byte RoleId { get; set; }

    public int CustId { get; set; }

    public int ProductId { get; set; }

    public int PageSize { get; set; }

    public int PageIndex { get; set; }

    public int TotalPages { get; set; }

    public string Language { get; set; }

    public int ProductGroupId { get; set; } = ProductGroupConstants.Sportsbook;

    public bool IsValid() => CustId > 0 & RoleId > 1 && RoleId < 5 && ProductId > 0;
}