namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class MasterPTSettingItem : GroupItemDefault
{
    private const string PTXmlFormat
        = "<R {0} IsLive=\"{1}\" SuperPT=\"{2}\" IsAuto=\"{3}\" AutoPT=\"{4}\" MasterPT=\"{5}\" MinPT=\"{6}\"/>";

    /// <summary>
    /// Super PT
    /// </summary>
    public decimal S3 { get; set; }

    /// <summary>
    /// Super Auto PT
    /// </summary>
    public decimal US3 { get; set; }

    /// <summary>
    /// Is Auto
    /// </summary>
    public bool IS3 { get; set; }

    /// <summary>
    /// Master Max PT
    /// </summary>
    public decimal M3 { get; set; }

    /// <summary>
    /// Master Min PT
    /// </summary>
    public decimal N3 { get; set; }

    public override string ToXml()
    {
        return string.Format(
            PTXmlFormat,
            base.ToXml(),
            IsLive,
            IS3 ? 0 : S3,
            IS3 ? 1 : 0,
            IS3 ? US3 : 0,
            M3,
            N3);
    }
}
