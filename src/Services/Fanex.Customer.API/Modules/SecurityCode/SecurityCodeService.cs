using Fanex.Customer.API.Modules.SecurityCode.Commands;
using Fanex.Customer.API.Modules.SecurityCode.Criteria;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SecurityCode;

public interface ISecurityCodeService
{
    string GenerateSalt(int custId);

    Task<Result> VerifySecurityCode(VerifySecurityCodeRequest param);

    Task<string> GetSecurityCode(int custId, int subAccountId);
}

public class SecurityCodeService : ISecurityCodeService
{
    private const string MD5 = "MD5";
    private const string SHA1 = "SHA1";
    private const string SECURITYCODEPADDING = "1qaw3+{:P)";

    private readonly IDynamicRepository dynamicRepository;

    public SecurityCodeService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public string GenerateSalt(int custId)
        => Encryption.ComputeHash(MD5, custId.ToString(), SECURITYCODEPADDING);

    public async Task<Result> VerifySecurityCode(VerifySecurityCodeRequest param)
    {
        var actualSecurityCode = await GetSecurityCode(param.CustId, param.SubAccId).ConfigureAwait(false);
        var salt = GenerateSalt(param.IsSubAccount() ? param.SubAccId : param.CustId);
        var hashedSecurityCode = Encryption.ComputeHash(SHA1, param.SecurityCode, salt);
        var isCorrectSecurityCode = string.Equals(actualSecurityCode, hashedSecurityCode);
        return await UpdateSecurityCodeCounter(param, isCorrectSecurityCode);
    }

    public Task<string> GetSecurityCode(int custId, int subAccountId)
        => dynamicRepository.GetAsync<string>(subAccountId.Equals(0)
            ? new GetMainAccountSecurityCodeCriteria(custId)
            : new GetSubAccountSecurityCodeCriteria(custId, subAccountId));

    private async Task<Result> UpdateSecurityCodeCounter(VerifySecurityCodeRequest param, bool isCorrectSecurityCode)
    {
        if (param.SubAccId.Equals(0))
        {
            var command = new UpdateMainAccountSecurityCodeCounterCommand(param.CustId, isCorrectSecurityCode, param.Language, param.IP);
            await dynamicRepository.ExecuteAsync(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }
        else
        {
            var command = new UpdateSubAccountSecurityCodeCounterCommand(param.CustId, param.SubAccId, isCorrectSecurityCode, param.Language, param.IP);
            await dynamicRepository.ExecuteAsync(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }
    }
}