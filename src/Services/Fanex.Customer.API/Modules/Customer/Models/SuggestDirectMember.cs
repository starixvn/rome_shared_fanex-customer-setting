namespace Fanex.Customer.API.Modules.Customer.Models;

public class SuggestDirectMember
{
    public SuggestDirectMember()
    { }

    public SuggestDirectMember(string userName, string firstName, string lastName, string userName2, int custId)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        UserName2 = userName2;
        CustId = custId;
    }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName2 { get; set; }

    public int CustId { get; set; }
}

public class SuggestCashMember : SuggestDirectMember
{ }