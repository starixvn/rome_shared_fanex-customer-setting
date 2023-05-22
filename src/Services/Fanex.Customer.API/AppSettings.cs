namespace Fanex.Customer.API;

public class AppSettings
{
    public string Y7SiteLabel { get; set; }

    public byte SettingStatus { get; set; }

    public byte CreationStatus { get; set; }

    public bool IsY7 (string siteLabel)
        => Y7SiteLabel.Contains(siteLabel);
}