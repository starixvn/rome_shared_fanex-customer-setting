using Fanex.Caching;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.CopyCustomer.Criteria;
using Fanex.Customer.API.Modules.CopyCustomer.Command;
using Fanex.Customer.API.Shared.Models;
using System.Collections;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface IBalanceCustomerService
{
    Task InitBalanceCustomer(int custId);
}

public class BalanceCustomerService : IBalanceCustomerService
{
    private readonly ICacheService cacheService;
    private readonly IDynamicRepository repository;

    public BalanceCustomerService(ICacheService cacheService, IDynamicRepository repository)
    {
        this.cacheService = cacheService;
        this.repository = repository;
    }

    public async Task InitBalanceCustomer(int custId)
    {
        var siteDB = await GetSiteDBConfig(custId) ?? throw new Exception($"CustId({custId}) not found site db information");
        var debitBalance = new InitBalanceCustomerCommand(custId, isCreditBalance: false, siteDB.PrefixSPName);
        var insertDebit = repository.ExecuteAsync(debitBalance);

        var creditBalance = new InitBalanceCustomerCommand(custId, isCreditBalance: true, siteDB.PrefixSPName);
        var insertCredit = repository.ExecuteAsync(creditBalance);
        await Task.WhenAll(insertDebit, insertCredit);

        if (debitBalance.ErrCode != 0 || creditBalance.ErrCode != 0)
        {
            var removeDebit = repository.ExecuteAsync(new RemoveBalanceCustomerCommand(custId, isCreditBalance: false, siteDB.PrefixSPName));
            var removeCredit = repository.ExecuteAsync(new RemoveBalanceCustomerCommand(custId, isCreditBalance: true, siteDB.PrefixSPName));
            await Task.WhenAll(removeDebit, removeCredit);
        }
    }

    private async Task<SiteDB> GetSiteDBConfig(int custId)
    {
        var siteDB = await cacheService.GetOrSetAsync(
            $"Fanex.Customer.API.Services.SiteDBConfigs-{custId}",
            () => repository.FetchAsync<SiteDB>(new GetSiteDBConfigCriteria(custId)),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return siteDB.FirstOrDefault();
    }
}