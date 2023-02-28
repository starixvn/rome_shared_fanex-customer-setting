using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CopyCustomer.DCS;


public interface IDCSService
{
    Task InsertMemberSettings(int custId, int sourceCustId, int actorId, int actorSubAccId, string ip);
}

public class DCSService : IDCSService
{
    private readonly IDynamicRepository repository;
    private readonly Logging.ILogger logger;
    private readonly AppSettings appSettings;

    public DCSService(
        IDynamicRepository repository,
        Logging.ILogger logger,
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

