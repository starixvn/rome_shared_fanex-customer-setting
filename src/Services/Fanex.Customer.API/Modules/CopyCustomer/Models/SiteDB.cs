namespace Fanex.Customer.API.Modules.CopyCustomer.Models;

public class SiteDB
{
    public int SiteDBGroupID { get; set; }

    public string LinkedServerName { get; set; }

    public string ClientAccessName { get; set; }

    public string AGIP { get; set; }

    public bool IsSiteDB => SiteDBGroupID != 0;

    public string PrefixSPName
    => IsSiteDB ? $"SiteDB{SiteDBGroupID}_" : string.Empty;

}