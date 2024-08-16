using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public class GetSiteDBConfigCriteria(string site) : CriteriaBase
    {
        public string Site { get; } = site;

        public override string GetSettingKey() => "Acc_ServerDBConfigSel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Site);
    }
}