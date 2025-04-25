using Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember.Criteria;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.RegisterCashMember;

public interface IRegisterCashMemberService
{
    Task<Result> RegisterCashMember(RegisterCashMemberCommand command);
}

public class RegisterCashMemberService: IRegisterCashMemberService
{
    private readonly IDynamicRepository _dynamicRepository;

    public RegisterCashMemberService(IDynamicRepository dynamicRepository)
    {
        _dynamicRepository = dynamicRepository;
    }

    public async Task<Result> RegisterCashMember(RegisterCashMemberCommand command)
    {
        // await _dynamicRepository.ExecuteAsync(command);
        // return new Result
        // {
        //     ErrCode = command.ErrCode,
        //     ErrMsg = command.ErrMsg
        // };
        return Result.CreateSuccess();
    }
}
