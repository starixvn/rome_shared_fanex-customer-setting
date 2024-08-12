using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Licensees.Criteria
{
    public class InsertBalanceExceptionCommand(string insBalExceptCustList) : NonQueryCommand
    {
        public string InsBalExceptCustList { get; } = insBalExceptCustList;

        public override string GetSettingKey() => "API_SiteBuilder_InsertBalance_Exception";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(InsBalExceptCustList);
    }
}