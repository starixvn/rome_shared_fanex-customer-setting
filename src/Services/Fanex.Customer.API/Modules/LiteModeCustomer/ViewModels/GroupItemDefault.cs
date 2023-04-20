namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;

public class GroupItemDefault
{
    public int SporttypeId { get; set; }

    public string SporttypeName { get; set; }

    public int BettypeId { get; set; }

    public string BettypeName { get; set; }

    public string DisplayName { get; set; }

    internal string ToXml()
    {
        throw new NotImplementedException();
    }
}