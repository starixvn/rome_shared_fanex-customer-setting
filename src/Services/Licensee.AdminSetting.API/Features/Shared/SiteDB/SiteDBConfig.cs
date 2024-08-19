namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public record SiteDBConfig
    {
        public int SiteDBGroupId { get; init; }

        public string LinkedServerName { get; init; }

        public string ClientAccessName { get; init; }

        public string AGIP { get; init; }

        public int CustDBGroupId { get; init; }

        public string LinkedServerNameDBCust { get; init; }

        public string ClientAccessNameDBCust { get; init; }

        public string AGIP_DBCust { get; init; }

        public string Site { get; init; }
    }
}