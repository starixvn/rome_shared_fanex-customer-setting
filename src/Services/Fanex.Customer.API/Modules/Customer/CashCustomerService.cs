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
    private readonly IDynamicRepository dynamicRepository;
    private readonly ISiteDBService siteDBService;
    private readonly IWriteLogSPService writeLogSPService;

    public CashCustomerService(
        IDynamicRepository dynamicRepository,
        ISiteDBService siteDBService,
        IWriteLogSPService writeLogSPService)
    {
        this.dynamicRepository = dynamicRepository;
        this.siteDBService = siteDBService;
        this.writeLogSPService = writeLogSPService;
    }

    public async Task<Result> UpdateEnableCashService(UpdateCashServiceStatusRequest request)
    {
        if (!IsValidRequest(request, out var validationError))
        {
            return Result.CreateError(validationError);
        }
        request.Language = LanguageUtils.ToLanguageKey(request.Language);
        var criteria = CreateUpdateCashServiceStatusCommand(request);
        var dataLog = await dynamicRepository.FetchAsync<LogDataEntity>(criteria);

        if (criteria.ErrCode != 0)
            return ResultFromCriteria(criteria);

        await WriteLogsAsync(dataLog);

        var initCmd = CreateInitializeCommand(request);
        await dynamicRepository.ExecuteAsync(initCmd);

        if (initCmd.ErrCode > 0)
            return ResultFromCriteria(initCmd);

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
                Fanex.Logging.LogService.Error($"Age_CashServiceBilling_DailyFee_Ins Error: {billingCmd.ErrCode}, Message: {billingCmd.ErrMsg}, Input: CustId={billingCmd.CustId}, TrialEndTime={billingCmd.TrialEndTime}, IP={billingCmd.IP}");
            }
        }

        return new Result(0, "Success");
    }

    private static UpdateCashServiceStatusCommand CreateUpdateCashServiceStatusCommand(UpdateCashServiceStatusRequest request) =>
        new(
            request.CustId,
            request.ServiceType,
            request.SubAccId,
            request.IP,
            request.Language
        );

    private static Result ResultFromCriteria(dynamic criteria) =>
        new(criteria.ErrCode, criteria.ErrMsg);

    private async Task WriteLogsAsync(IEnumerable<LogDataEntity> dataLog)
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
                log.Duration
            );
        }
    }

    private static AgeCashServiceCustServiceTypeInitializeCommand CreateInitializeCommand(UpdateCashServiceStatusRequest request) =>
        new(
            request.CustId,
            request.ServiceType,
            request.Site,
            request.SubAccId,
            request.IP,
            request.Language
        );

    private bool IsValidRequest(UpdateCashServiceStatusRequest request, out string error)
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
