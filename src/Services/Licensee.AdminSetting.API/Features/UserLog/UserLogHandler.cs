using System.Text;
using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.UserLog
{
    public interface IUserLogHandler
    {
        Task WriteLog(int custDBGroupId, LogParam param);
    }

    public class UserLogHandler(IDynamicRepository repository, Fanex.Logging.ILogger logger) : IUserLogHandler
    {
        private readonly IDynamicRepository repository = repository;
        private readonly Fanex.Logging.ILogger logger = logger;

        public async Task WriteLog(int custDBGroupId, LogParam param)
        {
            try
            {
                await repository.ExecuteAsync(new InsertUserLogMainCommand(
                    param.UserId,
                    param.CustId,
                    param.ActionId,
                    param.TrackingValue,
                    param.TrackingValue2)).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message, ex);

                await repository.ExecuteAsync(new InsertUserLogSiteDBCommand(
                    custDBGroupId,
                    param.UserId,
                    param.CustId,
                    param.ActionId,
                    param.TrackingValue,
                    param.TrackingValue2)).ConfigureAwait(false);
            }
        }
    }
}