namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class GroupItemDefault
{
    public GroupItemDefault() : this(0, 0, null) { }

    public GroupItemDefault(int sporttypeid, int bettypeid, string DisplayName)
    {
        this.SporttypeId = sporttypeid;
        this.BettypeId = bettypeid;
        this.DisplayName = DisplayName;
    }

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