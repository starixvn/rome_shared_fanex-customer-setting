using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command
{
    public class UpdateSubAccountPageAccessCriteria : NonQueryCommand
    {
        public int SubAccId { get; set; }

        public int CustId { get; set; }

        public string UpdatePageAccess { get; set; }

        public string Language { get; set; }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Age_CustSubUpdPageAccess";
        }

        public override bool IsValid()
        {
            return CustId > 0 && SubAccId > 0;
        }
    }
}