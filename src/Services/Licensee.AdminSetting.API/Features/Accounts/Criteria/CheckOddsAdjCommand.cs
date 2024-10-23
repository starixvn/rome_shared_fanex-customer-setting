using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Accounts.Criteria
{
    public class CheckOddsAdjCommand(string siteId) : NonQueryCommand
    {
        public string SiteId { get; } = siteId;

        public override string GetSettingKey() => "API_Dep_CustomerCheckOddsAdj";

        public override bool IsValid() => true;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}