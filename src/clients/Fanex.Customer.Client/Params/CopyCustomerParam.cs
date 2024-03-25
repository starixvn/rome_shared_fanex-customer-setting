using Fanex.Customer.Client.Constants;

namespace Fanex.Customer.Client.Params
{
    public class CopyMultipleCustomersParam : CopyCustomerParam
    {
        public CopyMultipleCustomersParam(
            int actorId,
            int actorSubAccId,
            int uplineId,
            byte custLevel,
            string actorUsername,
            string userName,
            string password,
            decimal credit,
            decimal maxCredit,
            int sourceCustId,
            int numberOfAccounts,
            string site,
            bool isQRCode,
            string ip,
            string language = "en-US",
            bool isInternal = false)
        : base(actorId, actorSubAccId, uplineId, custLevel, actorUsername, userName, password, credit, maxCredit, sourceCustId, site, isQRCode, ip, language, isInternal)
        {
            NumberOfAccounts = numberOfAccounts;
        }

        public int NumberOfAccounts { get; }
    }

    public class CopyCustomerParam
    {
        public CopyCustomerParam(
            int actorId,
            int actorSubAccId,
            int uplineId,
            byte custLevel,
            string actorUsername,
            string userName,
            string password,
            decimal credit,
            decimal maxCredit,
            int sourceCustId,
            string site,
            bool isQRCode,
            string ip,
            string language = Languages.English,
            bool isInternal = false)
        {
            ActorId = actorId;
            ActorSubAccId = actorSubAccId;
            UplineId = uplineId;
            CustLevel = custLevel;
            ActorUsername = actorUsername;
            Username = userName;
            Password = password;
            Credit = credit;
            MaxCredit = maxCredit;
            SourceCustId = sourceCustId;
            Site = site;
            IsQrCode = isQRCode;
            Language = language;
            IP = ip;
            IsInternal = isInternal;
        }

        public int ActorId { get; }

        public int ActorSubAccId { get; }

        public string ActorUsername { get; }

        public int UplineId { get; }

        public byte CustLevel { get; }

        public string Username { get; }

        public string Password { get; }

        public decimal Credit { get; }

        public decimal MaxCredit { get; }

        public int SourceCustId { get; }

        public string Site { get; }

        public bool IsQrCode { get; }

        public string Language { get; }

        public string IP { get; }

        public int SystemId { get; internal set; }

        public bool IsInternal { get; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string MobilePhone { get; set; } = string.Empty;

        public string Fax { get; set; } = string.Empty;
    }
}
