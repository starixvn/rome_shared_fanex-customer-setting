using Alpha.CloneAccount.Criteria.NG2;
using Alpha.CloneAccount.Params;
using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Services;

public interface IAPILogger
{
    Task<long> LogRequest(LogAPIRequestParam param);

    Task LogResponse(long logId, int returnCode, string response);
}

public class APILogger : IAPILogger
{
    private readonly IDynamicRepository dynamicRepository;

    public APILogger(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<long> LogRequest(LogAPIRequestParam param)
    {
        var command = new WriteLogExternalAPIInsertCommand(
            param.SystemId,
            param.ProductId,
            param.UserId,
            param.FunctionName,
            param.CustId,
            param.ActionId,
            param.Request.ToString());

        await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

        return command.LogId;
    }

    public async Task LogResponse(long logId, int returnCode, string response)
        => await dynamicRepository.ExecuteAsync(new WriteLogExternalAPIUpdResponseCommand(logId, returnCode, response)).ConfigureAwait(false);
}