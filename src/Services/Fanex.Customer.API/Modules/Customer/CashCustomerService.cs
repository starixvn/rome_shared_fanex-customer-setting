using Fanex.Customer.API.Modules.Customer.Criteria.CashService;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests.CashService;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer;

public interface ICashCustomerService
{
    Task<Result> UpdateEnableCashService(UpdateCashServiceStatusRequest request);
}

public class CashCustomerService : ICashCustomerService
{
    private const int ERR_CODE_NO_DOMAIN_AVAILABLE = 100;

    private readonly IDynamicRepository dynamicRepository;
    private readonly ISiteDBService siteDBService;
    private readonly IWriteLogSPService writeLogSPService;
    private readonly IWriteLogSPTransferService writeLogSPTransferService;
    private readonly IWriteLogSPCashService writeLogSPCashService;
    private readonly Fanex.Logging.ILogger _logger;

    public CashCustomerService(
        IDynamicRepository dynamicRepository,
        ISiteDBService siteDBService,
        IWriteLogSPService writeLogSPService,
        IWriteLogSPTransferService writeLogSPTransferService,
        IWriteLogSPCashService writeLogSPCashService,
        Fanex.Logging.ILogger logger)
    {
        this.dynamicRepository = dynamicRepository;
        this.siteDBService = siteDBService;
        this.writeLogSPService = writeLogSPService;
        this.writeLogSPTransferService = writeLogSPTransferService;
        this.writeLogSPCashService = writeLogSPCashService;
        this._logger = logger;
    }

    public async Task<Result> UpdateEnableCashService(UpdateCashServiceStatusRequest request)
    {
        if (!IsValidRequest(request, out var validationError))
        {
            return Result.CreateError(validationError);
        }

        request.Language = LanguageUtils.ToLanguageKey(request.Language);
        var siteDBConfig = await siteDBService.GetSiteDBConfigByCustId(request.CustId);
        int siteDBGroupId = siteDBConfig?.SiteDBGroupID ?? 0;
        var criteria = CreateUpdateCashServiceStatusCommand(request, siteDBGroupId);
        var updateDataLog = await dynamicRepository.FetchAsync<LogDataEntity>(criteria);

        if (criteria.ErrCode != 0)
            return ResultFromCriteria(criteria);


        var initCmd = CreateInitializeCommand(request);
        var initDataLog = await dynamicRepository.FetchAsync<LogDataEntity>(initCmd);

        if (initCmd.ErrCode > 0)
        {
            if (initCmd.ErrCode == ERR_CODE_NO_DOMAIN_AVAILABLE)
            {
                await _logger.ErrorAsync($"Age_CashService_CustServiceTypeInitialize returned ErrCode {ERR_CODE_NO_DOMAIN_AVAILABLE}: {initCmd.ErrMsg}");
            }
            else
            {
                var rollbackCmd = CreateUpdateCashServiceStatusCommand(
                    new UpdateCashServiceStatusRequest
                    {
                        CustId = request.CustId,
                        ServiceType = 0,
                        IP = request.IP,
                        Language = request.Language,
                        Site = request.Site
                    },
                    siteDBGroupId
                );
                await dynamicRepository.ExecuteAsync(rollbackCmd);
                return ResultFromCriteria(initCmd);
            }
        }

        await WriteLogsCashAsync(initDataLog, siteDBGroupId);
        await WriteLogsAsync(updateDataLog, siteDBGroupId);
        if (initCmd.TrialEndTime.HasValue)
        {
            var billingCmd = new AgeCashServiceBillingDailyFeeInsCommand(
                request.CustId,
                initCmd.TrialEndTime.Value,
                request.IP
            );
            await dynamicRepository.ExecuteAsync(billingCmd);

            if (billingCmd.ErrCode > 0)
            {
                await _logger.ErrorAsync($"Age_CashServiceBilling_DailyFee_Ins Error: {billingCmd.ErrCode}, Message: {billingCmd.ErrMsg}, Input: CustId={billingCmd.CustId}, TrialEndTime={billingCmd.TrialEndTime}, IP={billingCmd.IP}");
            }
            else
            {
                await writeLogSPTransferService.WriteLogSPTransferAsync(
                    billingCmd.LogData,
                    siteDBGroupId
                );
            }
        }

        return new Result(0, "Success");
    }

    internal static UpdateCashServiceStatusCommand CreateUpdateCashServiceStatusCommand(UpdateCashServiceStatusRequest request, int siteDBGroupId) =>
        new(
            request.CustId,
            request.ServiceType,
            request.IP,
            request.Language,
            siteDBGroupId
        );

    internal static Result ResultFromCriteria(dynamic criteria) =>
        new(criteria.ErrCode, criteria.ErrMsg);

    internal async Task WriteLogsAsync(IEnumerable<LogDataEntity> dataLog, int siteDBGroupId)
    {
        if (dataLog == null)
            return;

        foreach (var log in dataLog)
        {
            if (log == null || (log.UserID == 0 && log.CustID == 0 && log.ActionID == 0))
                continue;
            await writeLogSPService.WriteLogSPAsync(
                log.UserID,
                log.CustID,
                log.ActionID,
                log.TrackingValue,
                log.TrackingValue2,
                log.Duration,
                siteDBGroupId
            );
        }
    }

    internal async Task WriteLogsCashAsync(IEnumerable<LogDataEntity> dataLog, int siteDBGroupId)
    {
        if (dataLog == null)
            return;

        foreach (var log in dataLog)
        {
            if (log == null || (log.UserID == 0 && log.CustID == 0 && log.ActionID == 0))
                continue;
            await writeLogSPCashService.WriteLogSPCashAsync(
                log.UserID,
                log.CustID,
                log.ActionID,
                log.TrackingValue,
                log.TrackingValue2,
                log.TrackingValueMask,
                log.TrackingValueMask2,
                log.Duration,
                siteDBGroupId
            );
        }
    }

    internal static AgeCashServiceCustServiceTypeInitializeCommand CreateInitializeCommand(UpdateCashServiceStatusRequest request) =>
        new(
            request.CustId,
            request.ServiceType,
            request.Site,
            request.IP,
            request.Language
        );

    internal bool IsValidRequest(UpdateCashServiceStatusRequest request, out string error)
    {
        if (request == null)
        {
            error = "Request cannot be null.";
            return false;
        }
        if (request.CustId <= 0)
        {
            error = "Invalid CustId.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(request.Site))
        {
            error = "Site is required.";
            return false;
        }
        if (request.ServiceType != 0 && request.ServiceType != 1)
        {
            error = "ServiceType must be 0 (Credit) or 1 (Cash).";
            return false;
        }
        // IP and Language are not required
        error = null;
        return true;
    }
}
