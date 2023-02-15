namespace Alpha.CloneAccount.Tests.Services
{
    using System.Threading.Tasks;
    using Alpha.CloneAccount.Configurations;
    using CloneAccount.Criteria;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using CloneAccount.Services;
    using Constants;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    [Trait("Category", "Services")]
    public class CustomerServiceTests
    {
        private readonly IDynamicRepository repository;
        private readonly PasswordService passwordService;
        private readonly SportbooksCopyMemberSpParamsBuilder copyMemberSpParamsBuilder;
        private readonly ThirdPartyProductService thirdPartyProductService;
        private readonly CustomerService customerService;
        private readonly AppSettings appSettings;

        public CustomerServiceTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            passwordService = Substitute.For<PasswordService>();
            copyMemberSpParamsBuilder = Substitute.For<SportbooksCopyMemberSpParamsBuilder>();
            thirdPartyProductService = Substitute.For<ThirdPartyProductService>();
            appSettings = Substitute.For<AppSettings>();
            customerService = new CustomerServiceImpl(repository, passwordService, copyMemberSpParamsBuilder, thirdPartyProductService, appSettings);
        }

        [Fact]
        public async Task ValidateMaxNewMember_ValidCriteria_CallsExecuteAsync()
        {
            // Arrange
            var agentId = 33120484;
            var language = "en-US";

            // Act
            await customerService.ValidateMaxNewMember(agentId, language);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Any<ValidateMaxNewMemberCriteria>());
        }

        [Fact]
        public async Task GetPTInfo_ValidCriteria_CallsGetAsync()
        {
            // Arrange
            var custId = 33120484;

            // Act
            await customerService.GetSMAPTInfo(UserLevel.Super, custId);

            // Assert
            await repository
                .Received()
                .GetAsync<CustomerPTInfo>(Arg.Any<GetSMAPTInfoCriteria>());
        }

        [Fact]
        public async Task GetPTInfo_ValidCriteria_ReturnsCorrectPT()
        {
            // Arrange
            repository
                .GetAsync<CustomerPTInfo>(Arg.Any<ICriteria>())
                .Returns(new CustomerPTInfo(0, 0, "supertest1", 1000));

            // Act
            var actualPT = await customerService.GetSMAPTInfo(UserLevel.Super, 33120484);

            // Assert
            Assert.True(actualPT.CustId == 33120484 && actualPT.CustLevel == UserLevel.Super.Value);
        }

        [Fact]
        public async Task GetPTInfo_InvalidCriteria_DoNothing()
        {
            // Arrange
            var custId = 0;
            var custLevel = UserLevel.Agent;

            // Act
            await customerService.GetSMAPTInfo(custLevel, custId);

            // Assert
            await repository
                .DidNotReceive()
                .GetAsync<CustomerPTInfo>(new GetSMAPTInfoCriteria(custLevel, custId));
        }

        [Fact]
        public async Task GetPTInfo_InvalidCustId_ReturnsNull()
        {
            // Arrange
            var custId = 8;
            var custLevel = UserLevel.Super;
            CustomerPTInfo expectedResult = null;
            repository
                .GetAsync<CustomerPTInfo>(Arg.Any<ICriteria>())
                .Returns(expectedResult);

            // Act
            var actualResult = await customerService.GetSMAPTInfo(custLevel, custId);

            // Assert
            Assert.Null(actualResult);
        }

        [Fact]
        public async Task CopyCustomer_ValidMaster_CallsExecuteAsync()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000000,
                0,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                false);
            passwordService
                .Generate()
                .Returns("1234aaAA");

            // Act
            await customerService.CopyCustomer(param);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Any<CopyMasterCommand>());
        }

        [Fact]
        public async Task CopyCustomer_ValidAgent_CallsExecuteAsync()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(3),
                "agent",
                string.Empty,
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
            passwordService
                .Generate()
                .Returns("1234aaAA");

            // Act
            await customerService.CopyCustomer(param);

            // Assert
            await repository
                .Received()
                .ExecuteAsync(Arg.Any<CopyAgentCommand>());
        }

        [Fact]
        public async Task CheckReachLimitDownline_Always_CallsExecuteAsync()
        {
            // Arrange
            var param = new CheckReachLimitDownlineParam(1176070, 4, "supertest1", "en-US");

            // Act
            await customerService.CheckReachLimitDownline(param);

            // Assert
            await repository.Received().ExecuteAsync(Arg.Is<CheckReachLimitDownlineCommand>(
                c => c.CustId == param.CustId
                && c.RoleId == param.CustLevel
                && c.Prefix == param.Username
                && c.Language == param.LanguageKey));
        }

        [Fact]
        public async Task CheckLimitNewDownline_Always_CallsExecuteAsync()
        {
            // Arrange
            var param = new CheckLimitNewDownlineParam(1176070, UserLevel.Super, "en-US");

            // Act
            await customerService.CheckLimitNewDownline(param);

            // Assert
            await repository.Received().ExecuteAsync(Arg.Is<CheckLimitNewDownlineCommand>(c => c.CustId == param.CustId));
        }

        [Fact]
        public async Task CheckLimitNewDownline_SuperIsNotAllowed_ReturnsCorrectErrorMessage()
        {
            // Arrange
            var param = new CheckLimitNewDownlineParam(1176070, UserLevel.Super, "en-US");
            repository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(0));

            // Act
            var result = await customerService.CheckLimitNewDownline(param);

            // Assert
            Assert.Equal("Sorry, you can only create 3 masters per day, please try again tomorrow.", result.Message);
        }

        [Fact]
        public async Task CheckLimitNewDownline_MasterIsNotAllowed_ReturnsCorrectErrorMessage()
        {
            // Arrange
            var param = new CheckLimitNewDownlineParam(1176070, UserLevel.Master, "en-US");
            repository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(0));

            // Act
            var result = await customerService.CheckLimitNewDownline(param);

            // Assert
            Assert.Equal("Sorry, you can only create 3 agents per day, please try again tomorrow.", result.Message);
        }

        [Fact]
        public async Task CheckLimitNewDownline_AgentIsNotAllowed_ReturnsCorrectErrorMessage()
        {
            // Arrange
            var param = new CheckLimitNewDownlineParam(1176070, UserLevel.Agent, "en-US");
            repository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(0));

            // Act
            var result = await customerService.CheckLimitNewDownline(param);

            // Assert
            Assert.Equal("Sorry, you can only create 5 members per day, please try again tomorrow.", result.Message);
        }
    }
}