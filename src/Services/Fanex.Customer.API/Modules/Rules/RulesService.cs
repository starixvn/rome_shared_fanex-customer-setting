using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Rules
{
    public interface IRulesService
    {
        Task<Result> CheckLimitNewDownline(CheckAllLimitedRulesRequest request);

        Task<Result> CheckReachLimitDownline(CheckAllLimitedRulesRequest request);

        Task<Result> CheckMaxNewMemberBySuper(CheckAllLimitedRulesRequest request);

        Task<Result> CheckAllLimitedRules(CheckAllLimitedRulesRequest request);
    }

    public class RulesService : IRulesService
    {
        private readonly IDynamicRepository dynamicRepository;

        public RulesService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<Result> CheckLimitNewDownline(CheckAllLimitedRulesRequest request)
        {
            var command = new CheckLimitNewDownlineCommand(request.CustId, request.IsDirectMember);
            await dynamicRepository.ExecuteAsync(command);

            return command.IsAllowed
                ? Result.CreateSuccess()
                : GetLimitNewDownlineErrorResult(request);
        }

        public async Task<Result> CheckReachLimitDownline(CheckAllLimitedRulesRequest request)
        {
            var command = new CheckReachLimitDownlineCommand(request.CustId, request.CustLevel, request.ActorUsername, LanguageUtils.ToLanguageKey(request.Language));
            await dynamicRepository.ExecuteAsync(command);

            return command.ErrCode == 0
                ? Result.CreateSuccess()
                : Result.CreateError(command.ErrMsg);
        }

        public async Task<Result> CheckMaxNewMemberBySuper(CheckAllLimitedRulesRequest request)
        {
            var command = new CheckMaxNewMemberBySuperCommand(request.CustId, LanguageUtils.ToLanguageKey(request.Language));
            await dynamicRepository.ExecuteAsync(command);

            return command.ErrCode == 0
                ? Result.CreateSuccess()
                : Result.CreateError(command.ErrMsg);
        }

        public async Task<Result> CheckAllLimitedRules(CheckAllLimitedRulesRequest request)
        {
            var result = Result.CreateSuccess();

            if (request.CustLevel == UserLevel.Agent.Value || request.IsDirectMember)
            {
                result = await CheckMaxNewMemberBySuper(request).ConfigureAwait(false);
            }

            if (result.IsSuccessful)
            {
                result = await CheckReachLimitDownline(request).ConfigureAwait(false);
            }

            if (result.IsSuccessful)
            {
                result = await CheckLimitNewDownline(request).ConfigureAwait(false);
            }

            return result;
        }

        private static Result GetLimitNewDownlineErrorResult(CheckAllLimitedRulesRequest request)
        {
            const int LimitNewDownlineErrorCode = 1001;


            return new Result(
                LimitNewDownlineErrorCode,
                StaticResources.Get(GetLimitNewDownlineResourceKey(request), request.Language));
        }

        private static string GetLimitNewDownlineResourceKey(CheckAllLimitedRulesRequest request)
        {
            if (request.IsDirectMember)
            {
                return "LimitAgentNewDownlineError";
            }

            var custLevel = Enumeration.FromValue<UserLevel>(request.CustLevel);
            if (custLevel.IsSuperLevel())
            {
                return "LimitSuperNewDownlineError";
            }

            if (custLevel.IsMasterLevel())
            {
                return "LimitMasterNewDownlineError";
            }

            return "LimitAgentNewDownlineError";
        }
    }
}
