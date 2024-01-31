using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Password.Criteria
{
    public abstract class BaseCommand : NonQueryCommand
{
    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }
}
}