using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Password.Criteria
{
    public class UpdateLastChangePasswordIndirectAccountCriteria : NonQueryCommand
    {
        public int SubAccId { get; set; }

        public int CustId { get; set; }

        public int CustIdIndirect { get; set; }

        public int SubAccIdIndirect { get; set; }

        public string UplineUsername { get; set; }

        public string DownlineUsername { get; set; }

        public string Language { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Age_CustomerIndirectLoginAccountUpd";

        public override bool IsValid()
            => (CustId > 0 || SubAccId > 0) && (CustIdIndirect > 0 || SubAccIdIndirect > 0);
    }
}
