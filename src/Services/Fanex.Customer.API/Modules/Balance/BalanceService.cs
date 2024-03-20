using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Balance;

public interface IBalanceService
{
    Task InitBalance(string site, int custId);
}

public class BalanceService : IBalanceService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ISiteDBGroupService siteDBGroupService;

    public BalanceService(ISiteDBGroupService siteDBGroupService, IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
        this.siteDBGroupService = siteDBGroupService;
    }

    public async Task InitBalance(string site, int custId)
    {
        var siteDBName = await siteDBGroupService.GetSiteDBName(site);

        var creditBalance = new InitCreditBalanceCommand(custId, siteDBName);
        var insertCredit = dynamicRepository.ExecuteAsync(creditBalance);

        var depositBalance = new InitDepositBalanceCommand(custId, siteDBName);
        var insertDeposit = dynamicRepository.ExecuteAsync(depositBalance);
        await Task.WhenAll(insertCredit, insertDeposit);

        if (creditBalance.ErrCode != 0 || depositBalance.ErrCode != 0)
        {
            var removeDeposit = dynamicRepository.ExecuteAsync(new RemoveDepositBalanceCustomer(custId, siteDBName));
            var removeCredit = dynamicRepository.ExecuteAsync(new RemoveCreditBalanceCustomer(custId, siteDBName));
            await Task.WhenAll(removeDeposit, removeCredit);
        }
    }
}