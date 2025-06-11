namespace Fanex.Customer.API.Shared.Services;

using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Threading.Tasks;
using Fanex.Logging;


public interface IWriteLogSPService
{
    Task WriteLogSPAsync(
        int userId,
        int custId,
        int actionId,
        string trackingValue,
        string trackingValue2,
        int duration,
        int siteDBGroupId);
}

public class WriteLogSPService : IWriteLogSPService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ILogger _logger;

    public WriteLogSPService(IDynamicRepository dynamicRepository, ILogger logger)
    {
        this.dynamicRepository = dynamicRepository;
        this._logger = logger;
    }

    public async Task WriteLogSPAsync(
        int userId,
        int custId,
        int actionId,
        string trackingValue,
        string trackingValue2,
        int duration,
        int siteDBGroupId)
    {
        try
        {
            var dbLogCriteria = new WriteLogSPAtDBLOGCommand(userId, custId, actionId, trackingValue, trackingValue2, duration);
            await dynamicRepository.ExecuteAsync(dbLogCriteria);
            return;
        }
        catch (System.Exception ex)
        {
            await _logger.ErrorAsync(ex.ToString(), ex);
        }

        try
        {
            var criteria = new WriteLogSPCommand(userId, custId, actionId, trackingValue, trackingValue2, duration, siteDBGroupId);
            await dynamicRepository.ExecuteAsync(criteria);
        }
        catch (System.Exception ex)
        {
            await _logger.ErrorAsync(ex.ToString(), ex);
        }
    }

}

public class WriteLogSPAtDBLOGCommand : WriteLogSPCommand
{
    public WriteLogSPAtDBLOGCommand(
        int userId,
        int custId,
        int actionId,
        string trackingValue,
        string trackingValue2,
        int duration)
        : base(userId, custId, actionId, trackingValue, trackingValue2, duration, 0)
    {
    }
}

public class WriteLogSPCommand : NonQueryCommand
{
    public int UserID { get; set; }
    public int CustID { get; set; }
    public int ActionID { get; set; }
    public string TrackingValue { get; set; }
    public string TrackingValue2 { get; set; }
    public int Duration { get; set; }

    [SpParam(Ignored = true)]
    public int SiteDBGroupID { get; set; }

    public WriteLogSPCommand(int userId, int custId, int actionId, string trackingValue, string trackingValue2, int duration, int siteDBGroupId)
    {
        UserID = userId;
        CustID = custId;
        ActionID = actionId;
        TrackingValue = trackingValue;
        TrackingValue2 = trackingValue2;
        Duration = duration;
        SiteDBGroupID = siteDBGroupId;
    }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("WriteLog_UserLog_RunSP", SiteDBGroupID);
    public override bool IsValid() => true;
}
