namespace Alpha.CloneAccount.Tests.Services
{
    using System;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using ONEbook.NG2.API.Client.Services;
    using Xunit;

    [Trait("Category", "Services")]
    public class NG2ServiceTests
    {
        private readonly ING2StatusService ng2StatusService;
        private readonly ILogger logger;
        private readonly NG2Service ng2Service;

        // public NG2ServiceTests()
        // {
        //     ng2StatusService = Substitute.For<ING2StatusService>();
        //     logger = Substitute.For<ILogger>();
        //     ng2Service = new NG2ServiceImpl(ng2StatusService, logger);
        // }

        // [Fact]
        // public void SyncCustomerSettings_Hide_DoNothing()
        // {
        //     // Arrange
        //     var createdCustomer = new CustomerInfo(
        //         33120484,
        //         "1234aa",
        //         "master",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         33120480,
        //         0,
        //         0,
        //         3,
        //         false,
        //         true,
        //         false
        //         );
        //     var disableNG2 = true;
        //     var actor = new CustomerInfo(
        //         33120519,
        //         "1234aa",
        //         "super",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         0,
        //         0,
        //         0,
        //         4,
        //         true,
        //         false,
        //         true
        //         );
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(4),
        //         "master",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         33120481,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);

        //     // Act
        //     ng2Service.SyncCustomerSettings(createdCustomer, disableNG2, actor, param);

        //     // Assert
        //     ng2StatusService
        //         .DidNotReceive()
        //         .AutoInsertAccount(
        //             createdCustomer.CustId,
        //             createdCustomer.UserName,
        //             RoleId.Master,
        //             actor.CustId,
        //             disableNG2,
        //             SiteId.Alpha,
        //             actor.Currency,
        //             actor.CustId,
        //             param.SystemId,
        //             param.IP,
        //             param.Language,
        //             param.ActorSubAccId);
        // }

        // [Fact]
        // public void SyncCustomerSettings_NotSync_DoNothing()
        // {
        //     // Arrange
        //     var createdCustomer = new CustomerInfo(
        //         33120484,
        //         "1234aa",
        //         "master",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         33120480,
        //         0,
        //         0,
        //         3,
        //         false,
        //         true,
        //         false
        //         );
        //     var disableNG2 = true;
        //     var actor = new CustomerInfo(
        //         33120519,
        //         "1234aa",
        //         "super",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         0,
        //         0,
        //         0,
        //         4,
        //         false,
        //         false,
        //         true
        //         );
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(4),
        //         "master",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         33120481,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);

        //     // Act
        //     ng2Service.SyncCustomerSettings(createdCustomer, disableNG2, actor, param);

        //     // Assert
        //     ng2StatusService
        //         .DidNotReceive()
        //         .AutoInsertAccount(
        //             createdCustomer.CustId,
        //             createdCustomer.UserName,
        //             RoleId.Master,
        //             actor.CustId,
        //             disableNG2,
        //             SiteId.Alpha,
        //             actor.Currency,
        //             actor.CustId,
        //             param.SystemId,
        //             param.IP,
        //             param.Language,
        //             param.ActorSubAccId);
        // }

        // [Theory]
        // [InlineData(4)]
        // [InlineData(3)]
        // [InlineData(2)]
        // public void SyncCustomerSettings_Always_CallsAutoInsertAccount(byte actorUserLevel)
        // {
        //     // Arrange
        //     var createdCustomer = new CustomerInfo(
        //         33120484,
        //         "1234aa",
        //         "master",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         33120480,
        //         0,
        //         0,
        //         (byte)(actorUserLevel - 1),
        //         false,
        //         true,
        //         false
        //         );
        //     var disableNG2 = false;
        //     var actor = new CustomerInfo(
        //         33120519,
        //         "1234aa",
        //         "super",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         0,
        //         0,
        //         0,
        //         actorUserLevel,
        //         false,
        //         true,
        //         false
        //         );
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(actorUserLevel),
        //         "master",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         33120481,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);

        //     // Act
        //     ng2Service.SyncCustomerSettings(createdCustomer, disableNG2, actor, param);

        //     // Assert
        //     ng2StatusService
        //         .Received()
        //         .AutoInsertAccount(
        //             createdCustomer.CustId,
        //             createdCustomer.UserName,
        //             (RoleId)(actorUserLevel - 1),
        //             actor.CustId,
        //             disableNG2,
        //             SiteId.Alpha,
        //             actor.Currency,
        //             actor.CustId,
        //             param.SystemId,
        //             param.IP,
        //             param.Language,
        //             param.ActorSubAccId);
        // }

        // [Fact]
        // public void SyncCustomerSettings_ThrowException_CallsWriteLog()
        // {
        //     // Arrange
        //     var createdCustomer = new CustomerInfo(
        //         33120484,
        //         "1234aa",
        //         "master",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         33120480,
        //         0,
        //         0,
        //         3,
        //         false,
        //         true,
        //         false
        //         );
        //     var disableNG2 = false;
        //     var actor = new CustomerInfo(
        //         33120519,
        //         "1234aa",
        //         "super",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         21,
        //         false,
        //         false,
        //         0,
        //         0,
        //         0,
        //         4,
        //         false,
        //         true,
        //         false
        //         );
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(4),
        //         "master",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         33120481,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);
        //     var exceptionMessage = "Test Exception";
        //     var ex = new Exception(exceptionMessage);
        //     ng2StatusService
        //         .AutoInsertAccount(
        //             createdCustomer.CustId,
        //             createdCustomer.UserName,
        //             RoleId.Master,
        //             actor.CustId,
        //             disableNG2,
        //             SiteId.Alpha,
        //             actor.Currency,
        //             actor.CustId,
        //             param.SystemId,
        //             param.IP,
        //             param.Language,
        //             param.ActorSubAccId)
        //         .Throws(ex);

        //     // Act
        //     ng2Service.SyncCustomerSettings(createdCustomer, disableNG2, actor, param);

        //     // Assert
        //     logger.Received().Warn(ex.ToString());
        // }
    }
}