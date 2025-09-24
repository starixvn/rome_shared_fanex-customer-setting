using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.FullMode
{
    public class CreateMasterFullModeServiceTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ICustomerSettingHelper> _mockHelperService;
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AppSettings _appSettings;
        private readonly CreateMasterFullModeService _service;

        public CreateMasterFullModeServiceTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockHelperService = new Mock<ICustomerSettingHelper>();
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _appSettings = new AppSettings { SettingStatus = 1 };

            _service = new CreateMasterFullModeService(
                _mockMessageService.Object,
                _appSettings,
                _mockHelperService.Object,
                _mockDynamicRepository.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnFailure_WhenValidationFails()
        {
            // Arrange
            var request = CreateTestRequest();
            SetupValidationFailure(1001, "Validation failed");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal(1001, result.Code);
            Assert.Equal("Validation failed", result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenAllStepsSucceed()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateMasterSuccess();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Contains(newCustId, result.NewCustIds);
        }

        [Fact]
        public async Task Create_ShouldHandleMasterCreditLimits()
        {
            // Arrange
            var request = CreateTestRequest();
            request.Settings.GeneralInfo.MaxCredit = 100000; // Master level credit
            request.Settings.GeneralInfo.AgentMaxCredit = 50000;
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateMasterSuccess();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.True(result.IsSuccessful);
            VerifyMasterCreationCalled();
        }

        private CreateRequest CreateTestRequest()
        {
            return new CreateRequest
            {
                Context = new CreateRequestContext
                {
                    CreatorId = 1,
                    SystemId = 1,
                    Language = "en",
                    IP = "127.0.0.1",
                    SubAccId = 1,
                    CustDBGroupID = 1
                },
                Settings = new CustomerSetting
                {
                    GeneralInfo = new GeneralInfo
                    {
                        UserName = "testmaster",
                        Password = "password123",
                        FirstName = "Test",
                        LastName = "Master",
                        Site = "MAIN",
                        UplineCustId = 100,
                        MaxCredit = 100000,
                        AgentMaxCredit = 50000,
                        MemberMaxCredit = 10000,
                        RoleId = CustomerLevel.Master,
                        UplineRoleId = CustomerLevel.Super
                    },
                    ProductGroups = new List<ProductGroup>()
                }
            };
        }

        private void SetupValidationSuccess()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateMasterSetting>()))
                .Callback<ValidateMasterSetting>(criteria =>
                {
                    criteria.ErrCode = 0;
                    criteria.MsgType = 0;
                    criteria.ErrMsg = "";
                });

            _mockMessageService.Setup(x => x.TranslateMessage(0, "", It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });

            _mockHelperService.Setup(x => x.CreateSettingTypesJson(It.IsAny<IEnumerable<ProductGroup>>(), false))
                .Returns("{}");
        }

        private void SetupValidationFailure(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateMasterSetting>()))
                .Callback<ValidateMasterSetting>(criteria =>
                {
                    criteria.ErrCode = errorCode;
                    criteria.MsgType = 1;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(1, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });
        }

        private void SetupRegistrationSuccess(int custId)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<RegisterCustomerBeforeAddNew>()))
                .Callback<RegisterCustomerBeforeAddNew>(criteria =>
                {
                    criteria.CustId = custId;
                    criteria.ErrCode = 0;
                    criteria.ErrMsg = "";
                });
        }

        private void SetupCreateMasterSuccess()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateMasterCommand>()))
                .Callback<CreateMasterCommand>(criteria =>
                {
                    criteria.ErrCode = 0;
                    criteria.MsgType = 0;
                    criteria.ErrMsg = "";
                });
        }

        private void VerifyMasterCreationCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<CreateMasterCommand>()), Times.Once);
        }
    }
}