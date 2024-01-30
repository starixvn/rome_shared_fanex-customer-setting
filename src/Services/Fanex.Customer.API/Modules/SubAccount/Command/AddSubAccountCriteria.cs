using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command
{
    public class AddSubAccountCriteria : NonQueryCommand
    {
        public int SubAccId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string UserPwd { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string PageAccess { get; set; }

        public string Language { get; set; }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Age_CustSubIns";
        }

        public override bool IsValid()
        {
            return CustId > 0 && SubAccId <= 0;
        }
    }
}