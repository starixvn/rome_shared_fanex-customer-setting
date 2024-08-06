namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public record CustomerSiteDB
    {
        public int GroupId { get; init; }

        public string ServerName { get; init; }
    }
}