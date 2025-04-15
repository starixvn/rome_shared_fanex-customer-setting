using Fanex.Customer.Client.Constants;
using Fanex.Customer.Client.Models;

namespace Fanex.Customer.Client.Params
{
    public class CreateCustomerParam
    {
        public CreateCustomerParam(
            GeneralInfo generalInfo,
            string productGroups,
            int creatorId,
            int subAccId,
            string ip,
            string site,
            string siteLabel,
            int custDBGroupID,
            string group,
            int? noOfCreateAccount,
            CreationModes mode = CreationModes.LiteMode,
            string language = Languages.English,
            string uiBehavior = null)
        {
            GeneralInfo = generalInfo;
            ProductGroups = productGroups;
            UiBehavior = uiBehavior;
            CreatorId = creatorId;
            SubAccId = subAccId;
            Language = language;
            IP = ip;
            Site = site;
            SiteLabel = siteLabel;
            CustDBGroupID = custDBGroupID;
            Group = group;
            Mode = (byte)mode;
            NoOfCreateAccount = noOfCreateAccount;
        }

        public GeneralInfo GeneralInfo { get; }

        public string ProductGroups { get; }

        public string UiBehavior { get; }

        public int CreatorId { get; }

        public int SubAccId { get; }

        public string Language { get; }

        public string IP { get; }

        public int CustDBGroupID { get; }

        public byte Mode { get; }

        public string Site { get; }

        public string SiteLabel { get; }

        public string Group { get; }
        
        public int? NoOfCreateAccount { get; }
    }
}
