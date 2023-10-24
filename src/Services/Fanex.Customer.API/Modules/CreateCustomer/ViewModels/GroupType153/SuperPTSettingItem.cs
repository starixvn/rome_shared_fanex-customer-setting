namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class SuperPTSettingItem : GroupItemDefault
{
    private const string PTXmlFormat = "<R {0} IsLive=\"{1}\" SuperPT=\"{2}\" IsAuto=\"{3}\" MasterPT=\"{4}\" MinPT=\"{5}\"/>";

    public bool IsAuto { get; set; }

    public decimal S4 { get; set; }

    public decimal M4 { get; set; }

    public decimal N4 { get; set; }

    public override string ToXml()
     => string.Format(
                PTXmlFormat,
                base.ToXml(),
                IsLive,
                S4,
                IsAuto ? 1 : 0,
                M4,
                N4);

    public override bool Is3DecimalPTValue()
        => (S4 *10) % 10 != 0
            || (M4 *10) % 10 != 0
            || (N4 *10) % 10 != 0;
}
