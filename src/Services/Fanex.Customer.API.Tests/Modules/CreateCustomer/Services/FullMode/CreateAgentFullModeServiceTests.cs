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
    public class CreateAgentFullModeServiceTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ICustomerSettingHelper> _mockHelperService;
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AppSettings _appSettings;
        private readonly CreateAgentFullModeService _service;

        public CreateAgentFullModeServiceTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockHelperService = new Mock<ICustomerSettingHelper>();
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _appSettings = new AppSettings { SettingStatus = 1 };

            _service = new CreateAgentFullModeService(
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
        public async Task Create_ShouldReturnFailure_WhenRegistrationFails()
        {
            // Arrange
            var request = CreateTestRequest();
            SetupValidationSuccess();
            SetupRegistrationFailure(1002, "Registration failed");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal(1002, result.Code);
        }

        [Fact]
        public async Task Create_ShouldReturnFailure_WhenCreateAgentFails()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateAgentFailure(1003, "Create agent failed");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal(1003, result.Code);
            VerifyRollbackCalled();
        }

        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenAllStepsSucceed()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateAgentSuccess();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Contains(newCustId, result.NewCustIds);
        }

        [Fact]
        public async Task Create_ShouldHandleMaxCreditValidation()
        {
            // Arrange
            var request = CreateTestRequest();
            request.Settings.GeneralInfo.MaxCredit = -100; // Invalid credit
            SetupValidationFailure(1004, "Invalid max credit");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(1004, result.Code);
        }

        [Fact]
        public async Task Create_ShouldVerifySettingTypesJson()
        {
            // Arrange
            var request = CreateTestRequest();
            var expectedJson = "{\"productGroups\":[]}";
            _mockHelperService.Setup(x => x.CreateSettingTypesJson(It.IsAny<IEnumerable<ProductGroup>>(), false))
                .Returns(expectedJson);
            SetupValidationSuccess();
            SetupRegistrationSuccess(12345);
            SetupCreateAgentSuccess();

            // Act
            await _service.Create(request);

            // Assert
            _mockHelperService.Verify(x => x.CreateSettingTypesJson(request.Settings.ProductGroups, false), Times.AtLeastOnce);
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
                        UserName = "testagent",
                        Password = "password123",
                        FirstName = "Test",
                        LastName = "Agent",
                        Site = "MAIN",
                        Phone = "123456789",
                        MobilePhone = "987654321",
                        UplineCustId = 100,
                        MaxCredit = 10000,
                        MemberMaxCredit = 5000,
                        RoleId = CustomerLevel.Agent,
                        UplineRoleId = CustomerLevel.Master
                    },
                    ProductGroups = new List<ProductGroup>
                    {
                        new ProductGroup
                        {
                            Id = 1,
                            GroupTypes = new List<GroupType>()
                        }
                    }
                }
            };
        }

        private void SetupValidationSuccess()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateAgentSetting>()))
                .Callback<ValidateAgentSetting>(criteria =>
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
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateAgentSetting>()))
                .Callback<ValidateAgentSetting>(criteria =>
                {
                    criteria.ErrCode = errorCode;
                    criteria.MsgType = 1;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(1, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });

            _mockHelperService.Setup(x => x.CreateSettingTypesJson(It.IsAny<IEnumerable<ProductGroup>>(), false))
                .Returns("{}");
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

            _mockMessageService.Setup(x => x.TranslateMessage(0, "", It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });
        }

        private void SetupRegistrationFailure(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<RegisterCustomerBeforeAddNew>()))
                .Callback<RegisterCustomerBeforeAddNew>(criteria =>
                {
                    criteria.CustId = 0;
                    criteria.ErrCode = errorCode;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(0, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });
        }

        private void SetupCreateAgentSuccess()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateAgentCommand>()))
                .Callback<CreateAgentCommand>(criteria =>
                {
                    criteria.ErrCode = 0;
                    criteria.MsgType = 0;
                    criteria.ErrMsg = "";
                });

            _mockMessageService.Setup(x => x.TranslateMessage(0, "", It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });
        }

        private void SetupCreateAgentFailure(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateAgentCommand>()))
                .Callback<CreateAgentCommand>(criteria =>
                {
                    criteria.ErrCode = errorCode;
                    criteria.MsgType = 1;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(1, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });
        }

        private void VerifyRollbackCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<RollbackRegisterCustomer>()), Times.Once);
        }
    }
}
