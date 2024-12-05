using System.Globalization;
using System.Text.RegularExpressions;
using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using NicknameResource = Fanex.Customer.API.Modules.AppResources.Resources;

namespace Fanex.Customer.API.Modules.Customer
{
    public interface INicknameService
    {
        Task<Result> CreateNickname(CreateNicknameParam param, bool nickNameLoginOnly);
    }

    public class NicknameService : INicknameService
    {
        private const int Code = 100;
        private static readonly Regex NicknameRule = new Regex(@"^[a-zA-Z0-9]{5,20}$");

        private readonly IDynamicRepository dynamicRepository;

        public NicknameService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<Result> CreateNickname(CreateNicknameParam param, bool nickNameLoginOnly)
        {
            if (param.IsEnableUpdateNickname)
            {
                var result = await ValidateAndCreateNickname(param);

                if (!result.IsSuccessful)
                {
                    return result;
                }
            }

            return await UpdateNicknameLoginOnly(new UpdateNicknameLoginOnlyParam(param, nickNameLoginOnly));
        }

        private static bool IsValidNickName(string nickName)
            => NicknameRule.IsMatch(nickName);

        private async Task<Result> ValidateAndCreateNickname(CreateNicknameParam param)
        {
            if (IsValidNickName(param.Nickname))
            {
                Result result;
                if (param.IsSubAccount)
                {
                    var registerSubAccNicknameCommand = new RegisterSubAccNicknameCommand(param.CustId, param.Nickname, param.LanguageKey, param.CustDBGroupID);
                    await dynamicRepository.ExecuteAsync(registerSubAccNicknameCommand);
                    result = new Result(registerSubAccNicknameCommand.ErrCode, registerSubAccNicknameCommand.ErrMsg);
                }
                else
                {
                    var registerMainAccNicknameCommand = new RegisterNicknameCommand(param.CustId, param.Nickname, param.LanguageKey);
                    await dynamicRepository.ExecuteAsync(registerMainAccNicknameCommand);
                    result = new Result(registerMainAccNicknameCommand.ErrCode, registerMainAccNicknameCommand.ErrMsg);
                }                

                if (result.IsSuccessful)
                {
                    var command = param.IsSubAccount
                        ? new CreateSubAccountNicknameCommand(param)
                        : new CreateNicknameCommand(param);
                    var processResult = await Execute(command);
                    if (!processResult.IsSuccessful)
                    {
                        var rollbackNicknameCommand = new RollbackNicknameCommand(param.CustId, param.SubAccountId);
                        await dynamicRepository.ExecuteAsync(rollbackNicknameCommand);
                    }

                    return processResult;
                }

                return result;
            }

            return Result.Create(NicknameResource.ResourceManager.GetString("NicknameRule", new CultureInfo(LanguageUtils.ToLanguage(param.LanguageKey))), Code);
        }

        private Task<Result> UpdateNicknameLoginOnly(UpdateNicknameLoginOnlyParam param)
        {
            NicknameNonQueryCommand command = param.IsSubAccount
                ? new UpdateSubAccountNicknameLoginOnlyCommand(param)
                : new UpdateNicknameLoginOnlyCommand(param);

            return Execute(command);
        }

        private async Task<Result> Execute(NicknameNonQueryCommand command)
        {
            await dynamicRepository.ExecuteAsync(command);
            return Result.Create(
                command.ErrCode == 0 ? NicknameResource.ResourceManager.GetString("ConfiguredNicknameSuccessfully", new CultureInfo(LanguageUtils.ToLanguage(command.Language))) : command.ErrMsg,
                command.ErrCode);
        }
    }
}
