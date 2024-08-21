using Fanex.Customer.Client.Constants;
using Fanex.Customer.Client.Models;

namespace Fanex.Customer.Client.Params
{
    public class UpdateMultipleCustomerParam
    {
        public UpdateMultipleCustomerParam(
            string productGroups,
            int actorId,
            int subAccId,
            int custId,
            string userName,
            int roleId,
            int uplineRoleId,
            string ip,
            int custDBGroupID,
            string customerIds,
            string customerNames,
            string site,
            string siteLabel,
            string language = Languages.English)
        {
            ProductGroups = productGroups;
            ActorId = actorId;
            SubAccId = subAccId;
            CustId = custId;
            UserName = userName;
            RoleId = roleId;
            UplineRoleId = uplineRoleId;
            Ip = ip;
            CustDBGroupID = custDBGroupID;
            Site = site;
            SiteLabel = siteLabel;
            CustomerIds = customerIds;
            CustomerNames = customerNames;
            Language = language;
        }

        public GeneralInfo GeneralInfo { get; }

        public string ProductGroups { get; }

        public int ActorId { get; }

        public int SubAccId { get; }

        public int CustId { get; }

        public string UserName { get; }

        public int RoleId { get; }

        public int UplineRoleId { get; }

        public string Ip { get; }

        public int CustDBGroupID { get; }

        public string Language { get; }

        public string CustomerIds { get; }

        public string CustomerNames { get; }

        public string Site { get; }

        public string SiteLabel { get; }
    }
}