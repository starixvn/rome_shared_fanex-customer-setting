namespace Fanex.Customer.API.Modules.CopyCustomer.Keno.Models;

public class AgentEditMemberPositionTaking : PositionTakingBase
{
    public bool IsUpdateA1 { get; set; }

    public bool IsUpdateComm { get; set; }

    public bool IsDataChanged()
    {
        return IsUpdateA1 || IsUpdateComm;
    }
}