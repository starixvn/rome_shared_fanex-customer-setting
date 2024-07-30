namespace Fanex.Customer.API.Modules.CreateSubAcc.Requests;
public class CreateSubAccRequest
{
    public int CustId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PageAccess { get; set; }
    public int LoginSubAccId { get; set; }
    public string Language { get; set; }
    public int UserId { get; set; }
    public string IP { get; set; }

    public string Site { get; set; }

}