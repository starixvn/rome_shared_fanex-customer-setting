namespace Fanex.Customer.API.Modules.Password.Criteria
{
    public class SkipForceChangePasswordCommand : BaseCommand
    {
        public int CustId { get; set; }

        public int SubAccId { get; set; }

        public string LoginIP { get; set; }

        public string Language { get; set; }

        public override string GetSettingKey() => "Age_CustomerSkipForceUpdPwd";

        public override bool IsValid() => CustId > 0;
    }
}