namespace Fanex.Customer.API.Modules.Customer.Requests;

public class GetProductCategoriesRequest
{
    public int? CustId { get; set; }
    
    public int? CurrencyId { get; set; }
    
    public string Site { get; set; }
    
    public string SiteLabel { get; set; }

    public string Language { get; set; }
}