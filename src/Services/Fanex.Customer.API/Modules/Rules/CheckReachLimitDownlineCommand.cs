namespace Fanex.Customer.API.Modules.Rules
{
    using Fanex.Data.Repository;

    public class CheckReachLimitDownlineCommand : NonQueryCommand
    {
        public CheckReachLimitDownlineCommand(
            int custId,
            int roleId,
            string prefix,
            string language)
        {
            CustId = custId;
            RoleId = roleId;
            Prefix = prefix;
            Language = language;
        }

        public int CustId { get; }

        public int RoleId { get; }

        public string Prefix { get; }

        public string Language { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Age_CustomerCheckReachDownline";

        public override bool IsValid() => CustId > 0 && RoleId > 0;
    }
}
