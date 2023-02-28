namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

using System;
using System.Threading.Tasks;
using Fanex.Customer.API;
using Fanex.Data.Repository;
using Fanex.Logging;
using Microsoft.Extensions.Options;

public interface IDCSService
{
    Task InsertMemberSettings(int custId, int sourceCustId, int actorId, int actorSubAccId, string ip);
}

public class DCSService : IDCSService
{
    private readonly IDynamicRepository repository;
    private readonly ILogger logger;
    private readonly AppSettings appSettings;

    public DCSService(
        IDynamicRepository repository,
        ILogger logger,
        IOptions<AppSettings> appSettings)
    {
        this.repository = repository;
        this.logger = logger;
        this.appSettings = appSettings.Value;
    }

    public async Task InsertMemberSettings(int custId, int sourceCustId, int actorId, int actorSubAccId, string ip)
    {
        try
        {
            await repository.ExecuteAsync(
                new InsertMemberDCSSettingCommand(
                    custId,
                    sourceCustId,
                    actorId,
                    actorSubAccId,
                    ip,
                    appSettings.SettingStatus));
        }
        catch (Exception ex)
        {
            logger.Warn(ex.ToString());
        }
    }
}

