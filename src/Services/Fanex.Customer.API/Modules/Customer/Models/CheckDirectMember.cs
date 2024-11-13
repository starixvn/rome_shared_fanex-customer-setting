namespace Fanex.Customer.API.Modules.Customer.Models;

public class CheckDirectMember
{
    public CheckDirectMember(string userName, int custId, bool enabled)
    {
        UserName = userName;    
        CustId = custId;
        IsEnabled = enabled;
    }

    public string UserName {get;}

    public int CustId {get;}

    public bool IsEnabled {get;}
}
