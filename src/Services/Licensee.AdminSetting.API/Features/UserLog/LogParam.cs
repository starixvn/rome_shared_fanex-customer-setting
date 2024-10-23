namespace Licensee.AdminSetting.API.Features.UserLog
{
    public record LogParam
    {
        public int UserId { get; init; }

        public int CustId { get; init; }

        public int ActionId { get; init; }

        public string TrackingValue { get; init; }

        public string TrackingValue2 { get; init; }
    }
}