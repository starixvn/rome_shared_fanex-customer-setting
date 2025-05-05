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

    public string UserName { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string UserName2 { get; }

    public int CustId { get; }
}

public class SuggestCashMember : SuggestDirectMember
{ }