namespace Alpha.CloneAccount.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Alpha.CloneAccount.Configurations;
    using Alpha.CloneAccount.Repositories;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.Logging;
    using Fanex.RemotingRef;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    [Trait("Category", "Services")]
    public class KenoServiceTests
    {
        private readonly ILogger logger;
        private readonly KenoService kenoService;
        private readonly KenoConfiguration kenoConfiguration;
        private readonly IKenoRepository kenoRepository;
        private readonly IRemotingRefService remotingRefService;


        public KenoServiceTests()
        {
            logger = Substitute.For<ILogger>();
            kenoConfiguration = Substitute.For<KenoConfiguration>();
            kenoRepository = Substitute.For<IKenoRepository>();
            remotingRefService = Substitute.For<IRemotingRefService>();
            kenoService = new KenoServiceImpl(logger, kenoConfiguration, kenoRepository, remotingRefService);
        }

        // [Fact]
        // public void UpdateMemberSettings_DontShowSetting_DoNothing()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     var sourceCustId = 33120485;
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(2),
        //         "member",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         sourceCustId,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);

        //     // Act
        //     kenoService.UpdateMemberSettings(
        //         custId,
        //         sourceCustId,
        //         param);

        //     // Assert
        //     settingService
        //         .DidNotReceive()
        //         .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>());
        // }

        // [Fact]
        // public void UpdateMemberSettings_ShowSetting_CallsGetSettingInCaseOfAgentEditMember()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     var sourceCustId = 33120485;
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(2),
        //         "member",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         sourceCustId,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);

        //     // Act
        //     kenoService.UpdateMemberSettings(
        //         custId,
        //         sourceCustId,
        //         param);

        //     // Assert
        //     settingService
        //         .Received()
        //         .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>());
        // }

        // [Fact]
        // public void UpdateMemberSettings_ShowSetting_CallsUpdateMemberCustomerSettingInCaseOfAgentEditMember()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     var sourceCustId = 33120485;
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(2),
        //         "member",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         sourceCustId,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);
        //     settingService
        //         .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>())
        //         .Returns(new AgentEditMemberSettingViewModel
        //         {
        //             CustomerInformation = new CustomerInformation(),
        //             BetSettings = new List<BetSetting>(),
        //             PositionTakings = new List<AgentEditMemberPositionTaking>()
        //         });

        //     // Act
        //     kenoService.UpdateMemberSettings(
        //         custId,
        //         sourceCustId,
        //         param);

        //     // Assert
        //     settingService
        //         .Received()
        //         .UpdateMemberCustomerSettingInCaseOfAgentEditMember(Arg.Any<AgentEditMemberSettingViewModel>(), true);
        // }

        // [Fact]
        // public void UpdateMemberSettings_ThrowException_CallsWriteLog()
        // {
        //     // Arrange
        //     var custId = 33120484;
        //     var sourceCustId = 33120485;
        //     var param = new CopyCustomerParam(
        //         33120519,
        //         0,
        //         string.Empty,
        //         Enumeration.FromValue<UserLevel>(2),
        //         "member",
        //         "1234aa",
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         string.Empty,
        //         1000,
        //         1000000,
        //         sourceCustId,
        //         "sourceUsername",
        //         "en-US",
        //         string.Empty,
        //         SystemIdentity.Chatbot,
        //         false);
        //     var exceptionMessage = "Test Exception";
        //     settingService
        //         .GetSettingInCaseOfAgentEditMember(Arg.Any<CustomerInformationViewModel>())
        //         .Throws(new Exception(exceptionMessage));

        //     // Act
        //     kenoService.UpdateMemberSettings(
        //         custId,
        //         sourceCustId,
        //         param);

        //     // Assert
        //     logger.Received().Throws(new Exception(exceptionMessage));
        // }
    }
}
