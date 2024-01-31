namespace Fanex.Customer.API.Modules.Password.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateSubAccountPasswordCriteria : BaseCommand
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string UserPwd { get; set; }

        public string OldPwd { get; set; }

        public string LoginIP { get; set; }

        public string Language { get; set; }

        public override string GetSettingKey()
        {
            return "UpdateSubAccountPassword";
        }

        public override bool IsValid() => CustId > 0;
    }
}