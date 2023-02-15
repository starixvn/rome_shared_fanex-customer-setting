namespace Alpha.CloneAccount.Services
{
    using System;
    using System.Threading.Tasks;
    using Alpha.CloneAccount.Configurations;
    using Criteria;
    using Fanex.Data.Repository;
    using Fanex.Logging;

    public interface DCSService
    {
        Task InsertMemberSettings(
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            string ip);
    }

    public class DCSServiceImpl : DCSService
    {
        private readonly IDynamicRepository repository;
        private readonly ILogger logger;
        private readonly AppSettings appSettings;

        public DCSServiceImpl(
            IDynamicRepository repository,
            ILogger logger,
            AppSettings aopSettings)
        {
            this.repository = repository;
            this.logger = logger;
            this.appSettings = aopSettings;
        }

        public async Task InsertMemberSettings(
            int custId,
            int sourceCustId,
            int actorId,
            int actorSubAccId,
            string ip)
        {
            try
            {
                await repository.ExecuteAsync(
                    new InsertMemberDCSSettingCommand(
                        custId,
                        sourceCustId,
                        actorId,
                        actorSubAccId,
                        ip,
                        appSettings.SettingStatus));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.ToString());
            }
        }
    }
}
