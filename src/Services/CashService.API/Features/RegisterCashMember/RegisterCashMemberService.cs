using CashService.API.Features.RegisterCashMember.Criteria;
using CashService.API.Models;
using Fanex.Data.Repository;

namespace CashService.API.Features.RegisterCashMember;

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
        return Result.Success;
    }
}
