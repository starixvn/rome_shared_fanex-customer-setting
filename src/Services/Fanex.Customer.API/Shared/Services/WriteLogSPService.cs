namespace Fanex.Customer.API.Shared.Services;

using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Threading.Tasks;


public interface IWriteLogSPService
{
    Task WriteLogSPAsync(
        int userId,
        int custId,
        int actionId,
        string trackingValue,
        string trackingValue2,
        int duration);
}

public class WriteLogSPService : IWriteLogSPService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ISiteDBService siteDBService;

    public WriteLogSPService(IDynamicRepository dynamicRepository, ISiteDBService siteDBService)
    {
        this.dynamicRepository = dynamicRepository;
        this.siteDBService = siteDBService;
    }

    public async Task WriteLogSPAsync(
        int userId,
        int custId,
        int actionId,
        string trackingValue,
        string trackingValue2,
        int duration)
    {
        try
        {
            var dbLogCriteria = new WriteLogSPAtDBLOGCommand(userId, custId, actionId, trackingValue, trackingValue2, duration);
            await dynamicRepository.ExecuteAsync(dbLogCriteria);
            return;
        }
        catch
        {
            // Do nothing
        }

        try
        {
            var siteDBConfig = await siteDBService.GetSiteDBConfigByCustId(custId);
            int siteDBGroupId = siteDBConfig?.SiteDBGroupID ?? 0;
            var criteria = new WriteLogSPCommand(userId, custId, actionId, trackingValue, trackingValue2, duration, siteDBGroupId);
            await dynamicRepository.ExecuteAsync(criteria);
        }
        catch
        {
            // Do nothing
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
