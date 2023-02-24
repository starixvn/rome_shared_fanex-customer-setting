namespace Fanex.Customer.API.Modules.Customer.PositionTaking;

public class GetCustomerPTRequest
{
    public int CustId { get; set; }

    public byte CustLevel { get; set; }

    public bool IsValid()
        => CustId > 0 && 1 < CustLevel && CustLevel < 5;
}