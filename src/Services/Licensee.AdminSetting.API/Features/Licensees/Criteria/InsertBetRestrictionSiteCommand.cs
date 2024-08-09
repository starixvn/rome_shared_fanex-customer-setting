using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class InsertBetRestrictionSiteCommand(int custDBGroupId, int siteId) : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int CustDBGroupId { get; } = custDBGroupId;

        public int SiteId { get; } = siteId;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(CustDBGroupId, "NAP_BetRestriction_SiteIns");

        public override bool IsValid() => SiteId > 0;
    }
}