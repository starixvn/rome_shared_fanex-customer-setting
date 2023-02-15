namespace Alpha.CloneAccount.Tests.Services
{
    using System.Threading.Tasks;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using NSubstitute;
    using Xunit;

    [Trait("Category", "Services")]
    public class ThirdPartyProductServiceTests
    {
        private readonly NG2Service ng2Service;
        private readonly DCSService dcsService;
        private readonly KenoService kenoService;
        private readonly ThirdPartyProductService thirdPartyProductService;

        public ThirdPartyProductServiceTests()
        {
            ng2Service = Substitute.For<NG2Service>();
            dcsService = Substitute.For<DCSService>();
            kenoService = Substitute.For<KenoService>();
            thirdPartyProductService = new ThirdPartyProductServiceImpl(ng2Service, dcsService, kenoService);
        }

        [Fact]
        public void SyncMasterOrAgentSettings_Always_CallsSync3rdProductSettings()
        {
            // Arrange
            var createdCustomer = new CustomerInfo(
                33120484,
                "1234aa",
                "master",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120519,
                0,
                0,
                3,
                false,
                true,
                false
                );
            var sourceCustomer = new CustomerInfo(
                33120480,
                "1234aa",
                "sourcemaster",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120519,
                0,
                0,
                3,
                false,
                true,
                false
                );
            var actor = new CustomerInfo(
                33120519,
                "1234aa",
                "super",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                0,
                0,
                0,
                4,
                false,
                true,
                false
                );
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                false);

            // Act
            thirdPartyProductService.SyncMasterOrAgentSettings(createdCustomer, sourceCustomer, actor, param);

            // Assert
            ng2Service
                .Received()
                .SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
        }

        [Fact]
        public async Task SyncMemberSettings_Always_CallsSync3rdProductMemberSettings()
        {
            // Arrange
            var createdCustomer = new CustomerInfo(
                33120484,
                "1234aa",
                "master",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120519,
                0,
                0,
                3,
                false,
                true,
                false
                );
            var sourceCustomer = new CustomerInfo(
                33120480,
                "1234aa",
                "sourcemaster",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                33120519,
                0,
                0,
                3,
                false,
                true,
                false
                );
            var actor = new CustomerInfo(
                33120519,
                "1234aa",
                "super",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                21,
                false,
                false,
                0,
                0,
                0,
                4,
                false,
                true,
                false
                );
            var param = new CopyCustomerParam(
                33120519,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                false);

            // Act
            await thirdPartyProductService.SyncMemberSettings(createdCustomer, sourceCustomer, actor, param);

            // Assert
            await dcsService
                .Received()
                .InsertMemberSettings(
                    createdCustomer.CustId,
                    sourceCustomer.CustId,
                    actor.CustId,
                    param.ActorSubAccId,
                    param.IP);

            kenoService
                .Received()
                .UpdateMemberSettings(
                    createdCustomer.CustId,
                    sourceCustomer.CustId,
                    param);

            ng2Service
                .Received()
                .SyncCustomerSettings(createdCustomer, sourceCustomer.DisableNG2, actor, param);
        }
    }
}