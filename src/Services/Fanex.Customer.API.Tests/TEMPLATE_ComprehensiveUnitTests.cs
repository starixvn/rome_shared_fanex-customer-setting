// TEMPLATE FOR COMPREHENSIVE UNIT TESTS
// Replace [SERVICE_NAME] with actual service name (e.g., CreateMemberFullMode, CreateAgentLiteMode, etc.)
// Replace [COMMAND_TYPE] with actual command type (e.g., CreateMemberCommand, CreateAgentCommand, etc.)
// Replace [VALIDATE_TYPE] with actual validation type (e.g., ValidateMemberSetting, ValidateAgentSetting, etc.)

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode; // or LiteMode
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.FullMode // or LiteMode
{
    public class [SERVICE_NAME]ServiceTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ICustomerSettingHelper> _mockHelperService;
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AppSettings _appSettings;
        private readonly [SERVICE_NAME]Service _service;

        public [SERVICE_NAME]ServiceTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockHelperService = new Mock<ICustomerSettingHelper>();
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _appSettings = new AppSettings { SettingStatus = 1 };

            _service = new [SERVICE_NAME]Service(
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
        public async Task Create_ShouldReturnFailure_WhenCreate[ENTITY]Fails()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreate[ENTITY]Failure(1003, "Create [entity] failed");

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
            SetupCreate[ENTITY]Success();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Contains(newCustId, result.NewCustIds);
        }

        [Fact]
        public async Task Create_ShouldHandle[ENTITY]SpecificLogic()
        {
            // Arrange - Add specific test for the entity type
            var request = CreateTestRequest();
            // Add entity-specific setup here
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreate[ENTITY]Success();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.True(result.IsSuccessful);
            Verify[ENTITY]CreationCalled();
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
                        UserName = "test[entity]",
                        Password = "password123",
                        FirstName = "Test",
                        LastName = "[Entity]",
                        Site = "MAIN",
                        UplineCustId = 100,
                        // Add entity-specific properties here
                        RoleId = CustomerLevel.[ENTITY_LEVEL],
                        UplineRoleId = CustomerLevel.[UPLINE_LEVEL]
                    },
                    ProductGroups = new List<ProductGroup>()
                }
            };
        }

        private void SetupValidationSuccess()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<[VALIDATE_TYPE]>()))
                .Callback<[VALIDATE_TYPE]>(criteria =>
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
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<[VALIDATE_TYPE]>()))
                .Callback<[VALIDATE_TYPE]>(criteria =>
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

        private void SetupCreate[ENTITY]Success()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<[COMMAND_TYPE]>()))
                .Callback<[COMMAND_TYPE]>(criteria =>
                {
                    criteria.ErrCode = 0;
                    criteria.MsgType = 0;
                    criteria.ErrMsg = "";
                });

            _mockMessageService.Setup(x => x.TranslateMessage(0, "", It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });
        }

        private void SetupCreate[ENTITY]Failure(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<[COMMAND_TYPE]>()))
                .Callback<[COMMAND_TYPE]>(criteria =>
                {
                    criteria.ErrCode = errorCode;
                    criteria.MsgType = 1;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(1, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });
        }

        private void Verify[ENTITY]CreationCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<[COMMAND_TYPE]>()), Times.Once);
        }

        private void VerifyRollbackCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<RollbackRegisterCustomer>()), Times.Once);
        }
    }
}