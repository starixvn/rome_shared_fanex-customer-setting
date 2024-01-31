namespace Fanex.Customer.API.Modules.Password.Criteria
{
    public class ResetMainAccountPasswordCriteria : BaseCommand
    {
        public int CustId { get; set; }

        public string UserPwd { get; set; }

        public int UserId { get; set; }

        public string Language { get; set; }

        public int SubAccId { get; set; }

        public string LoginIP { get; set; }

        public override string GetSettingKey()
        {
            return "ResetMainAccountPassword";
        }

        public override bool IsValid() => CustId > 0;
    }
}