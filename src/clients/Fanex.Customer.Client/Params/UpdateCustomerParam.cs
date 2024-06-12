using Fanex.Customer.Client.Constants;
using Fanex.Customer.Client.Models;

namespace Fanex.Customer.Client.Params
{
    public class UpdateCustomerParam
    {
        public UpdateCustomerParam(
            GeneralInfo generalInfo,
            string productGroups,
            int actorId,
            int subAccId,
            int custId,
            string userName,
            int roleId,
            int uplineRoleId,
            string ip,
            string site,
            string siteLabel,
            string language = Languages.English)
        {
            GeneralInfo = generalInfo;
            ProductGroups = productGroups;
            ActorId = actorId;
            SubAccId = subAccId;
            CustId = custId;
            UserName = userName;
            RoleId = roleId;
            UplineRoleId = uplineRoleId;
            Ip = ip;
            Site = site;
            SiteLabel = siteLabel;
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

        public string Language { get; }

        public string Site { get; }

        public string SiteLabel { get; }
    }
}