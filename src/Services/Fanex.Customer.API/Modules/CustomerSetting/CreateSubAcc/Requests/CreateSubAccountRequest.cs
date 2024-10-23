namespace Fanex.Customer.API.Modules.CreateSubAcc.Requests;
public class CreateSubAccountRequest : BaseSubAccountRequest
{
    public int RoleId { get; set; }
    public string NickName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PageAccess { get; set; }
    public int LoginSubAccId { get; set; }
    public int UserId { get; set; }
}