using Fanex.Customer.API.Modules.Password.Model;

namespace Fanex.Customer.API;

public interface IAppSettings
{
    byte SettingStatus { get; }
    string SiteLabelBelonging777Group { get; }
    string SpecialSites { get; }
    string SupportPT3DecimalSites { get; }
    string SitesAllowUncheckStakeLevelOnMember { get; }
    PasswordService PasswordService { get; }

    bool Belonging777Group(string siteLabel);
    bool IsSpecialSites(string siteLabel);
    bool IsSupportPT3Decimal(string siteLabel);
    bool IsAllowUncheckStakeLevelOnMember(string siteName);
}
