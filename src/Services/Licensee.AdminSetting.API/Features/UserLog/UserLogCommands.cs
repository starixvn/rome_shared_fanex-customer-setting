using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.UserLog
{
    public abstract class InsertUserLogBaseCommand(
        int userId,
        int custId,
        int actionId,
        string trackingValue,
        string trackingValue2)
        : NonQueryCommand
    {
        public int UserId { get; } = userId;

        public int CustId { get; } = custId;

        public int ActionId { get; } = actionId;

        public string TrackingValue { get; } = trackingValue;

        public string TrackingValue2 { get; } = trackingValue2;

        public override bool IsValid() => UserId > 0 && CustId > 0 && ActionId > 0 && !string.IsNullOrWhiteSpace(TrackingValue);
    }

    public class InsertUserLogMainCommand(int userId, int custId, int actionId, string trackingValue, string trackingValue2)
        : InsertUserLogBaseCommand(userId, custId, actionId, trackingValue, trackingValue2)
    {
        public override string GetSettingKey() => "Main_WriteLog_UserLog_RunSP";
    }

    public class InsertUserLogSiteDBCommand(int custDBGroupId, int userId, int custId, int actionId, string trackingValue, string trackingValue2)
        : InsertUserLogBaseCommand(userId, custId, actionId, trackingValue, trackingValue2)
    {
        [SpParam(Ignored = true)]
        public int CustDBGroupId { get; } = custDBGroupId;

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(CustDBGroupId, "WriteLog_UserLog_RunSP");
    }
}