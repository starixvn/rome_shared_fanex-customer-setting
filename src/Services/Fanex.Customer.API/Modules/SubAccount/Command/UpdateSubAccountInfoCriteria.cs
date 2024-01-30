using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command
{
    public class UpdateSubAccountInfoCriteria : NonQueryCommand
    {
        public int SubAccId { get; set; }

        public int CustId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public bool Closed { get; set; }

        public string Language { get; set; }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Age_CustSubUpdInfo";
        }

        public override bool IsValid()
        {
            return CustId > 0 && SubAccId > 0;
        }
    }
}