namespace Fanex.Customer.API;

public class AppSettings
{
    public byte SettingStatus { get; set; }

    public string SiteLabelBelonging777Group { get; set; }

    public bool Belonging777Group(string siteLabel)
    => !string.IsNullOrWhiteSpace(SiteLabelBelonging777Group) && SiteLabelBelonging777Group.Split(',').Select(site => site.Trim()).Any(site => site.Equals(siteLabel, System.StringComparison.CurrentCultureIgnoreCase));

    public string SpecialSites { get; set; }

    public bool IsSpecialSites(string siteLabel)
        => string.IsNullOrWhiteSpace(SpecialSites) ?
        false :
        SpecialSites.Split(',').Select(site => site.Trim()).Any(site => site.Equals(siteLabel, System.StringComparison.CurrentCultureIgnoreCase));

    public string SupportPT3DecimalSites { get; set; }

    public bool IsSupportPT3Decimal(string siteLabel)
        => string.IsNullOrWhiteSpace(SupportPT3DecimalSites) ?
        false :
        SupportPT3DecimalSites.Split(',').Select(site => site.Trim()).Any(site => site.Equals(siteLabel, System.StringComparison.CurrentCultureIgnoreCase));
        
    public string SitesAllowUncheckStakeLevelOnMember { get; set; }

    public bool IsAllowUncheckStakeLevelOnMember(string siteName)
        => string.IsNullOrWhiteSpace(SitesAllowUncheckStakeLevelOnMember) ?
        false :
        SitesAllowUncheckStakeLevelOnMember.Split(',').Select(site => site.Trim()).Any(site => site.Equals(siteName, System.StringComparison.CurrentCultureIgnoreCase));
}