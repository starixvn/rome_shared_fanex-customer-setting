namespace Fanex.Customer.API.Modules.CreateSubAcc.Requests;
public class BaseSubAccountRequest
{
    public int CustId { get; set; }
    public string Language { get; set; }    
    public string IP { get; set; }
    public string UserName { get; set; }
    public string NickName { get; set; }
    public string Site {get; set;}
}