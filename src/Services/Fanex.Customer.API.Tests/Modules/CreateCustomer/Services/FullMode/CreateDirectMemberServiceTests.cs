using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.FullMode
{
    public class CreateDirectMemberServiceTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ICustomerSettingHelper> _mockHelperService;
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AppSettings _appSettings;
        private readonly CreateDirectMemberService _service;

        public CreateDirectMemberServiceTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockHelperService = new Mock<ICustomerSettingHelper>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _appSettings = new AppSettings { SettingStatus = 1 };

            _service = new CreateDirectMemberService(
                _mockMessageService.Object,
                _appSettings,
                _mockHelperService.Object,
                _mockCustomerService.Object,
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
        public async Task Create_ShouldReturnSuccess_WhenAllStepsSucceed()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateDirectMemberSuccess();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Contains(newCustId, result.NewCustIds);
        }

        [Fact]
        public async Task Create_ShouldHandleDirectMemberSpecificLogic()
        {
            // Arrange
            var request = CreateTestRequest();
            request.Settings.GeneralInfo.UplineRoleId = CustomerLevel.Super; // Direct member under Super
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateDirectMemberSuccess();

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.True(result.IsSuccessful);
            VerifyDirectMemberCreationCalled();
        }

        [Fact]
        public async Task Create_ShouldRollbackOnFailure()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateDirectMemberFailure(1003, "Create member failed");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.False(result.IsSuccessful);
            VerifyRollbackCalled();
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
                        UserName = "directmember",
                        Password = "password123",
                        FirstName = "Direct",
                        LastName = "Member",
                        Site = "MAIN",
                        Phone = "123456789",
                        MobilePhone = "987654321",
                        UplineCustId = 100,
                        RoleId = CustomerLevel.Member,
                        UplineRoleId = CustomerLevel.Super // Direct member
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
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateDirectMemberSetting>()))
                .Callback<ValidateDirectMemberSetting>(criteria =>
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
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateDirectMemberSetting>()))
                .Callback<ValidateDirectMemberSetting>(criteria =>
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

        private void SetupCreateDirectMemberSuccess()
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateDirectMemberCommand>()))
                .Callback<CreateDirectMemberCommand>(criteria =>
                {
                    criteria.ErrCode = 0;
                    criteria.MsgType = 0;
                    criteria.ErrMsg = "";
                });

            _mockMessageService.Setup(x => x.TranslateMessage(0, "", It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });
        }

        private void SetupCreateDirectMemberFailure(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateDirectMemberCommand>()))
                .Callback<CreateDirectMemberCommand>(criteria =>
                {
                    criteria.ErrCode = errorCode;
                    criteria.MsgType = 1;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(1, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });
        }

        private void VerifyDirectMemberCreationCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<CreateDirectMemberCommand>()), Times.Once);
        }

        private void VerifyRollbackCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<RollbackRegisterCustomer>()), Times.Once);
        }
    }
}
