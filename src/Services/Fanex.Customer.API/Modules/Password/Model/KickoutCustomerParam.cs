using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.Password.Model
{
    public class KickoutCustomerParam
    {
        public KickoutCustomerParam(
            int custId,
            UserLevel custLevel,
            int serverId,
            string ip,
            int subAccountId = 0)
        {
            CustId = custId;
            CustLevel = custLevel;
            ServerId = serverId;
            IP = ip;
            SubAccountId = subAccountId;
        }

        public int CustId { get; }

        public UserLevel CustLevel { get; }

        public int ServerId { get; }

        public string IP { get; }

        public int SubAccountId { get; }

    }
}