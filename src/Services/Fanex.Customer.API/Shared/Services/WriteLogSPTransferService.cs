namespace Fanex.Customer.API.Shared.Services;

using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Logging;
using System.Threading.Tasks;

public interface IWriteLogSPTransferService
{
    Task WriteLogSPTransferAsync(string logData, int siteDBGroupId);
}

public class WriteLogSPTransferService : IWriteLogSPTransferService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ILogger _logger;

    public WriteLogSPTransferService(IDynamicRepository dynamicRepository, ILogger logger)
    {
        this.dynamicRepository = dynamicRepository;
        this._logger = logger;
    }

    public async Task WriteLogSPTransferAsync(string logData, int siteDBGroupId)
    {
        try
        {
            var dbLogCriteria = new WriteLogSPTransferAtDBLOGCommand(logData);
            await dynamicRepository.ExecuteAsync(dbLogCriteria);
            return;
        }
        catch (System.Exception ex)
        {
            await _logger.ErrorAsync(ex.ToString());
        }

        try
        {
            var criteria = new WriteLogSPTransferCommand(logData, siteDBGroupId);
            await dynamicRepository.ExecuteAsync(criteria);
        }
        catch (System.Exception ex)
        {
            await _logger.ErrorAsync(ex.ToString());
        }
    }
}

public class WriteLogSPTransferAtDBLOGCommand : WriteLogSPTransferCommand
{
    public WriteLogSPTransferAtDBLOGCommand(string logData)
        : base(logData, 0)
    {
    }
}

public class WriteLogSPTransferCommand : NonQueryCommand
{
    public string LogData { get; }

    [SpParam(Ignored = true)]
    public int SiteDBGroupID { get; set; }

    public WriteLogSPTransferCommand(string logData, int siteDBGroupId)
    {
        LogData = logData;
        SiteDBGroupID = siteDBGroupId;
    }

    public override string GetSettingKey() => SPNameUtils.GetSPNameWithPrefix("WriteLog_CashService_UserLog_Transfer", SiteDBGroupID);
    public override bool IsValid() => true;
}
