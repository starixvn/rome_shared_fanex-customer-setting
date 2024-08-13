using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class InsertBalanceExceptionCommand(string insBalExceptCustList, int siteDBGroupId) : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int SiteDBGroupId { get; } = siteDBGroupId;

        public string InsBalExceptCustList { get; } = insBalExceptCustList;

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(SiteDBGroupId, "API_SiteBuilder_InsertBalance_Exception");

        public override bool IsValid() => !string.IsNullOrWhiteSpace(InsBalExceptCustList);
    }
}