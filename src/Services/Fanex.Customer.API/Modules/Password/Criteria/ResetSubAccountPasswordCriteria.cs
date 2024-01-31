namespace Fanex.Customer.API.Modules.Password.Criteria
{
    public class ResetSubAccountPasswordCriteria : BaseCommand
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string UserPwd { get; set; }

        public string Language { get; set; }

        public int LoginSubAccId
        {
            get
            {
                //// Hard code because store procedured do nothing with this param
                return 0;
            }
        }

        public string LoginIP { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "ResetSubAccountPassword";
        }

        public override bool IsValid() => CustId > 0;
    }
}