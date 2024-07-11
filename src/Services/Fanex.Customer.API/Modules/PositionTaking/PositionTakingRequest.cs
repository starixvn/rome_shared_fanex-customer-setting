namespace Fanex.Customer.API.Modules.PositionTaking;

public class PositionTakingRequest
{
    public byte RoleId { get; set; }

    public int CustId { get; set; }

    public int ProductId { get; set; }

    public byte SettingStatus { get; set; }

    public bool IsValid() => CustId > 0 & RoleId > 1 && RoleId < 5 && ProductId > 0;
}