namespace Fanex.Customer.API.Modules.Password.Criteria
{
    public class UpdateMainAccountPasswordCriteria : BaseCommand
    {
        public int CustId { get; set; }

        public string UserPwd { get; set; }

        public string OldPwd { get; set; }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public string Language { get; set; }

        public override string GetSettingKey()
        {
            return "UpdateMainAccountPassword";
        }

        public override bool IsValid() => CustId > 0;
    }
}