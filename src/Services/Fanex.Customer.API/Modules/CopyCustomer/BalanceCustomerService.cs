using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CopyCustomer.Command;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface IBalanceCustomerService
{
    Task InitBalanceCustomer(int custId);
}

public class BalanceCustomerService : IBalanceCustomerService
{
    private readonly IDynamicRepository repository;

    public BalanceCustomerService(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task InitBalanceCustomer(int custId)
    {
        var debitBalance = new InitBalanceCustomerCommand(custId, isCreditBalance: false);
        var insertDebit = repository.ExecuteAsync(debitBalance);

        var creditBalance = new InitBalanceCustomerCommand(custId, isCreditBalance: true);
        var insertCredit = repository.ExecuteAsync(creditBalance);
        await Task.WhenAll(insertDebit, insertCredit);

        if (debitBalance.ErrCode != 0 || creditBalance.ErrCode != 0)
        {
            var removeDebit = repository.ExecuteAsync(new RemoveBalanceCustomerCommand(custId, isCreditBalance: false));
            var removeCredit = repository.ExecuteAsync(new RemoveBalanceCustomerCommand(custId, isCreditBalance: true));
            await Task.WhenAll(removeDebit, removeCredit);
        }
    }
}