namespace Licensee.AdminSetting.API.Features.UserLog
{
    public record LogParam(int UserId, int CustId, int ActionId, string TrackingValue, string TrackingValue2);
}