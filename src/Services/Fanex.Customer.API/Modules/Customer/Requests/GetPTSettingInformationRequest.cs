namespace Fanex.Customer.API.Modules.Customer.Requests;

public class GetPTSettingInformationRequest
{
    public int? CustId { get; set; }

    public bool IsTest { get; set; }
    
    public string Site { get; set; }

    public string Language { get; set; }
}
