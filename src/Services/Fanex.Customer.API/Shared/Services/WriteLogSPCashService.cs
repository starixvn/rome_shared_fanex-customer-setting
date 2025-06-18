using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Shared.Services
{
    public interface IWriteLogSPCashService
    {
        Task WriteLogSPCashAsync(
            int userId,
            int custId,
            int actionId,
            string trackingValue,
            string trackingValue2,
            string trackingValueMask,
            string trackingValueMask2,
            int duration,
            int siteDBGroupId);
    }

    public class WriteLogSPCashService : IWriteLogSPCashService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly Fanex.Logging.ILogger _logger;

        public WriteLogSPCashService(IDynamicRepository dynamicRepository, Fanex.Logging.ILogger logger)
        {
            this.dynamicRepository = dynamicRepository;
            this._logger = logger;
        }

        public async Task WriteLogSPCashAsync(
            int userId,
            int custId,
            int actionId,
            string trackingValue,
            string trackingValue2,
            string trackingValueMask,
            string trackingValueMask2,
            int duration,
            int siteDBGroupId)
        {
            try
            {
                var dbLogCriteria = new WriteLogSPCashAtDBLOGCommand(userId, custId, actionId, trackingValue, trackingValue2, trackingValueMask, trackingValueMask2, duration);
                await dynamicRepository.ExecuteAsync(dbLogCriteria);
                return;
            }
            catch (System.Exception ex)
            {
                await _logger.ErrorAsync(ex.ToString(), ex);
            }

            try
            {
                var criteria = new WriteLogSPCashCommand(userId, custId, actionId, trackingValue, trackingValue2, trackingValueMask, trackingValueMask2, duration, siteDBGroupId);
                await dynamicRepository.ExecuteAsync(criteria);
            }
            catch (System.Exception ex)
            {
                await _logger.ErrorAsync(ex.ToString(), ex);
            }
        }
    }

    public class WriteLogSPCashAtDBLOGCommand : WriteLogSPCashCommand
    {
        public WriteLogSPCashAtDBLOGCommand(
            int userId,
            int custId,
            int actionId,
            string trackingValue,
            string trackingValue2,
            string trackingValueMask,
            string trackingValueMask2,
            int duration)
            : base(userId, custId, actionId, trackingValue, trackingValue2, trackingValueMask, trackingValueMask2, duration, 0)
        {
        }
    }

    public class WriteLogSPCashCommand : NonQueryCommand
    {
        public int UserID { get; set; }
        public int CustID { get; set; }
        public int ActionID { get; set; }
        public string TrackingValue { get; set; }
        public string TrackingValue2 { get; set; }
        public string TrackingValueMask { get; set; }
        public string TrackingValueMask2 { get; set; }
        public int Duration { get; set; }

        [SpParam(Ignored = true)]
        public int SiteDBGroupID { get; set; }

        public WriteLogSPCashCommand(
            int userId,
            int custId,
            int actionId,
            string trackingValue,
            string trackingValue2,
            string trackingValueMask,
            string trackingValueMask2,
            int duration,
            int siteDBGroupId)
        {
            UserID = userId;
            CustID = custId;
            ActionID = actionId;
            TrackingValue = trackingValue;
            TrackingValue2 = trackingValue2;
            TrackingValueMask = trackingValueMask;
            TrackingValueMask2 = trackingValueMask2;
            Duration = duration;
            SiteDBGroupID = siteDBGroupId;
        }

        public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("WriteLog_CashService_UserLog_RunSP", SiteDBGroupID);
        public override bool IsValid() => true;
    }
}
