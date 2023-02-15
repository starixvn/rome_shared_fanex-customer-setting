using Alpha.CloneAccount.Models;
using Alpha.CloneAccount.Utils;

namespace Alpha.CloneAccount.Params
{
    public class CopyCustomerParam
    {
        public CopyCustomerParam(
            int actorId,
            int actorSubAccId,
            string actorSubAccName,
            UserLevel actorUserLevel,
            string userName,
            string password,
            string firstName,
            string lastName,
            string phone,
            string mobilePhone,
            string fax,
            decimal credit,
            decimal maxCredit,
            int sourceCustId,
            string sourceUsername,
            string language,
            string ip,
            byte systemId,
            bool isQrCode)
        {
            IsQrCode = isQrCode;
            ActorId = actorId;
            ActorSubAccId = actorSubAccId;
            ActorSubAccName = actorSubAccName;
            ActorUserLevel = actorUserLevel;

            UserName = userName;
            Password = password;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Phone = phone ?? string.Empty;
            MobilePhone = mobilePhone ?? string.Empty;
            Fax = fax ?? string.Empty;
            Credit = credit;
            MaxCredit = maxCredit;
            SourceCustId = sourceCustId;
            SourceUsername = sourceUsername;

            Language = language ?? "en-US";
            IP = ip ?? string.Empty;
            SystemId = systemId;
        }

        public bool IsQrCode { get; }

        public int ActorId { get; }

        public int ActorSubAccId { get; }

        public string ActorSubAccName { get; }

        public UserLevel ActorUserLevel { get; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Phone { get; }

        public string MobilePhone { get; }

        public string Fax { get; }

        public decimal Credit { get; }

        public decimal MaxCredit { get; }

        public int SourceCustId { get; }

        public string SourceUsername { get; }

        public string Language { get; }

        public string LanguageKey => LanguageUtils.ToLanguageKey(Language);

        public string IP { get; }

        public byte SystemId { get; }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetUserName(string userName)
        {
            UserName = userName;
        }
    }
}