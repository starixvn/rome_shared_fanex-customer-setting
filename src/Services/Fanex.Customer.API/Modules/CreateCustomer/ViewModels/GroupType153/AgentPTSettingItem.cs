namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;

public class AgentPTSettingItem : GroupItemDefault
{private const string PTXmlFormat
            = "<R {0} MasterPT=\"{1}\" IsAutoMasterPT=\"{2}\" AutoMasterPT=\"{3}\" AgentPT=\"{4}\" MinPT=\"{5}\"/>";

        /// <summary>
        /// Master PT
        /// </summary>
        public decimal M2 { get; set; }

        /// <summary>
        /// Master Auto PT
        /// </summary>
        public decimal UM2 { get; set; }

        /// <summary>
        /// Is Auto
        /// </summary>
        public bool IM2 { get; set; }

        /// <summary>
        /// Agent Max PT
        /// </summary>
        public decimal A2 { get; set; }

        /// <summary>
        /// Agent Min PT
        /// </summary>
        public decimal N2 { get; set; }

        /// <summary>
        /// Master Max PT
        /// </summary>
        public decimal M3 { get; set; }

        /// <summary>
        /// Master Min PT
        /// </summary>
        public decimal N3 { get; set; }

        /// <summary>
        /// Agent Max PT
        /// </summary>
        public decimal A4 { get; set; }

    public override string ToXml()
    {
            return string.Format(
                PTXmlFormat,
                base.ToXml(),
                IM2 ? 0 : M2,
                IM2 ? 1 : 0,
                IM2 ? UM2 : 0,
                A2,
                N2);
    }
}
