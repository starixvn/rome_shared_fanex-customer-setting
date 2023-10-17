namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class MemberPTItem : GroupItemDefault
{
    private const string PositionTakingXmlFormat = "<R {0} IsLive=\"{1}\" AgentPT=\"{2}\"/>";

    /// <summary>
    /// Agent PT
    /// </summary>
    public decimal A1 { get; set; }

    /// <summary>
    /// Agent Max PT
    /// </summary>
    public decimal A2 { get; set; }

    /// <summary>
    /// Agent Min PT
    /// </summary>
    public decimal N2 { get; set; }

    public override string ToXml()
    {
        return string.Format(
            PositionTakingXmlFormat,
            base.ToXml(),
            IsLive,
            A1);
    }
}