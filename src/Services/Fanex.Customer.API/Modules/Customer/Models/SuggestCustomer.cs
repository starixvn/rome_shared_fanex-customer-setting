namespace Fanex.Customer.API.Modules.Customer.Models;

public class SuggestCustomer
{
    public SuggestCustomer(string userName, string firstName, string lastName, string userName2)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        UserName2 = userName2;
    }

    public string UserName { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string UserName2 { get; }
}