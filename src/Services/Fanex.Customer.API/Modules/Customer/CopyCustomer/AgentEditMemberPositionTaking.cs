namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class AgentEditMemberPositionTaking : PositionTakingBase
{
    public bool IsUpdateA1 { get; set; }

    public bool IsUpdateComm { get; set; }

    public bool IsDataChanged()
    {
        return this.IsUpdateA1 || this.IsUpdateComm;
    }
}