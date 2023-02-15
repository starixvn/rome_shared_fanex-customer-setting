namespace Alpha.CloneAccount.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using Alpha.CloneAccount.Configurations;
    using CloneAccount.Criteria;
    using CloneAccount.Services;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    [Trait("Category", "Services")]
    public class DCSServiceTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ILogger logger;
        private readonly DCSService dcsService;
        private readonly AppSettings appSettings;

        public DCSServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            logger = Substitute.For<ILogger>();
            appSettings = Substitute.For<AppSettings>();
            dcsService = new DCSServiceImpl(dynamicRepository, logger, appSettings);
        }

        [Fact]
        public async Task InsertMemberSettings_ValidCriteria_CallsExecuteAsync()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var actorId = 33120519;
            var actorSubAccId = 0;
            var ip = string.Empty;

            // Act
            await dcsService.InsertMemberSettings(
                custId,
                sourceCustId,
                actorId,
                actorSubAccId,
                ip);

            // Assert
            await dynamicRepository
                .Received()
                .ExecuteAsync(Arg.Is<InsertMemberDCSSettingCommand>(c =>
                    c.CustId == custId
                    && c.SourceCustId == sourceCustId
                    && c.ActorId == actorId
                    && c.ActorSubAccId == actorSubAccId
                    && c.IP == ip));
        }

        [Fact]
        public async Task InsertMemberSettings_InvalidCriteria_CallsExecuteAsync()
        {
            // Arrange
            var custId = -1;
            var sourceCustId = -1;
            var actorId = -1;
            var actorSubAccId = -1;
            var ip = string.Empty;

            // Act
            await dcsService.InsertMemberSettings(
                custId,
                sourceCustId,
                actorId,
                actorSubAccId,
                ip);

            // Assert
            await dynamicRepository
                .DidNotReceive()
                .ExecuteAsync(new InsertMemberDCSSettingCommand(
                    custId,
                    sourceCustId,
                    actorId,
                    actorSubAccId,
                    ip,
                     1));
        }

        [Fact]
        public async Task InsertMemberSettings_ThrowException_CallsWriteLog()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var actorId = 33120519;
            var actorSubAccId = 0;
            var ip = string.Empty;
            var exceptionMessage = "Test Exception";
            var ex = new Exception(exceptionMessage);
            dynamicRepository
                .ExecuteAsync(Arg.Any<InsertMemberDCSSettingCommand>())
                .Throws(ex);

            // Act
            await dcsService.InsertMemberSettings(
                custId,
                sourceCustId,
                actorId,
                actorSubAccId,
                ip);

            // Assert
            logger.Received().Warn(ex.ToString());
        }

        [Fact]
        public async Task InsertMemberSettings_ThrowAggregateException_CallsWriteLog()
        {
            // Arrange
            var custId = 33120484;
            var sourceCustId = 33120485;
            var actorId = 33120519;
            var actorSubAccId = 0;
            var ip = string.Empty;
            var exceptionMessage = "Test Exception";
            var ex = new AggregateException(exceptionMessage);
            dynamicRepository
                .ExecuteAsync(Arg.Any<InsertMemberDCSSettingCommand>())
                .Throws(ex);

            // Act
            await dcsService.InsertMemberSettings(
                custId,
                sourceCustId,
                actorId,
                actorSubAccId,
                ip);

            // Assert
            logger.Received().Warn(ex.ToString());
        }
    }
}
