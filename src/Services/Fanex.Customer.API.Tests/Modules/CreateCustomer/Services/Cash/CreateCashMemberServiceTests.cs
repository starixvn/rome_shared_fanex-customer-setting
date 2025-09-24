using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.Cash;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.Cash
{
    public class CreateCashMemberServiceTests
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ICustomerSettingHelper> _mockHelperService;
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly AppSettings _appSettings;
        private readonly CreateCashMemberService _service;

        public CreateCashMemberServiceTests()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockHelperService = new Mock<ICustomerSettingHelper>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _appSettings = new AppSettings { SettingStatus = 1 };

            _service = new CreateCashMemberService(
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
            VerifyValidationCalled();
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
            Assert.Equal("Registration failed", result.Message);
            VerifyRegistrationCalled();
        }

        [Fact]
        public async Task Create_ShouldRetryRegistration_WhenCode201AndRetryCountUnderLimit()
        {
            // Arrange
            var request = CreateTestRequest();
            SetupValidationSuccess();
            SetupRegistrationFailureWithRetry(201, "Username exists");
            SetupSuggestUsernameSuccess("001");

            // Act
            var result = await _service.Create(request);

            // Assert
            VerifyRegistrationCalledMultipleTimes();
            _mockCustomerService.Verify(x => x.GetSuggestUsername(It.IsAny<GetSuggestUsernameRequest>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Create_ShouldReturnFailure_WhenCreateMemberFails()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateMemberFailure(1003, "Create member failed");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal(1003, result.Code);
            Assert.Equal("Create member failed", result.Message);
            VerifyRollbackCalled(newCustId);
        }

        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenAllStepsSucceed()
        {
            // Arrange
            var request = CreateTestRequest();
            var newCustId = 12345;
            request.Context.NoOfCreateAccount = 1;
            SetupValidationSuccess();
            SetupRegistrationSuccess(newCustId);
            SetupCreateMemberSuccess(newCustId);

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Contains(newCustId, result.NewCustIds);
            Assert.Contains(request.Settings.GeneralInfo.UserName, result.NewUserNames);
        }

        [Fact]
        public async Task Create_ShouldCreateMultipleAccounts_WhenNoOfCreateAccountIsGreaterThanOne()
        {
            // Arrange
            var request = CreateTestRequest();
            request.Context.NoOfCreateAccount = 2;
            var newCustId1 = 12345;
            var newCustId2 = 12346;
            
            SetupValidationSuccess();
            SetupRegistrationSuccessSequential(new[] { newCustId1, newCustId2 });
            SetupCreateMemberSuccess(newCustId1);
            SetupCreateMemberSuccess(newCustId2);
            SetupSuggestUsernameSuccess("001");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Equal(2, result.NewCustIds.Count);
            Assert.Contains(newCustId1, result.NewCustIds);
            Assert.Contains(newCustId2, result.NewCustIds);
        }

        [Fact]
        public async Task Create_ShouldHandleSuggestUsernameFailure()
        {
            // Arrange
            var request = CreateTestRequest();
            request.Context.NoOfCreateAccount = 2;
            SetupValidationSuccess();
            SetupSuggestUsernameFailure(1004, "Cannot suggest username");

            // Act
            var result = await _service.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Equal(1004, result.Code);
            Assert.Equal("Cannot suggest username", result.Message);
        }

        private CreateRequest CreateTestRequest()
        {
            return new CreateRequest
            {
                Context = new CreateRequestContext
                {
                    NoOfCreateAccount = 1,
                    CreatorId = 1,
                    CustDBGroupID = 1,
                    SystemId = 1,
                    Language = "en",
                    IP = "127.0.0.1",
                    SubAccId = 1
                },
                Settings = new CustomerSetting
                {
                    GeneralInfo = new GeneralInfo
                    {
                        UserName = "testuser",
                        Password = "password123",
                        FirstName = "Test",
                        LastName = "User",
                        Site = "MAIN",
                        Phone = "123456789",
                        MobilePhone = "987654321",
                        UplineCustId = 100,
                        UplineUserName = "upline",
                        UplineRoleId = 3,
                        RoleId = 4
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
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateCashMemberSetting>()))
                .Callback<ValidateCashMemberSetting>(criteria =>
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
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<ValidateCashMemberSetting>()))
                .Callback<ValidateCashMemberSetting>(criteria =>
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

        private void SetupRegistrationSuccessSequential(int[] custIds)
        {
            var sequence = _mockDynamicRepository.SetupSequence(x => x.ExecuteAsync(It.IsAny<RegisterCustomerBeforeAddNew>()));
            
            foreach (var custId in custIds)
            {
                var localCustId = custId; // Capture for closure
                sequence = sequence.Callback<RegisterCustomerBeforeAddNew>(criteria =>
                {
                    criteria.CustId = localCustId;
                    criteria.ErrCode = 0;
                    criteria.ErrMsg = "";
                });
            }

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

        private void SetupRegistrationFailureWithRetry(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.SetupSequence(x => x.ExecuteAsync(It.IsAny<RegisterCustomerBeforeAddNew>()))
                .Callback<RegisterCustomerBeforeAddNew>(criteria =>
                {
                    criteria.CustId = 0;
                    criteria.ErrCode = errorCode;
                    criteria.ErrMsg = errorMessage;
                })
                .Callback<RegisterCustomerBeforeAddNew>(criteria =>
                {
                    criteria.CustId = 12345;
                    criteria.ErrCode = 0;
                    criteria.ErrMsg = "";
                });

            _mockMessageService.Setup(x => x.TranslateMessage(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });
        }

        private void SetupCreateMemberSuccess(int custId)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateCashMemberCommand>()))
                .Callback<CreateCashMemberCommand>(criteria =>
                {
                    criteria.ErrCode = 0;
                    criteria.MsgType = 0;
                    criteria.ErrMsg = "";
                });

            _mockMessageService.Setup(x => x.TranslateMessage(0, "", It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = "Success" });
        }

        private void SetupCreateMemberFailure(int errorCode, string errorMessage)
        {
            _mockDynamicRepository.Setup(x => x.ExecuteAsync(It.IsAny<CreateCashMemberCommand>()))
                .Callback<CreateCashMemberCommand>(criteria =>
                {
                    criteria.ErrCode = errorCode;
                    criteria.MsgType = 1;
                    criteria.ErrMsg = errorMessage;
                });

            _mockMessageService.Setup(x => x.TranslateMessage(1, errorMessage, It.IsAny<string>()))
                .Returns(new TranslatedMessage { MainMessage = errorMessage });
        }

        private void SetupSuggestUsernameSuccess(string suffix)
        {
            _mockCustomerService.Setup(x => x.GetSuggestUsername(It.IsAny<GetSuggestUsernameRequest>()))
                .ReturnsAsync(new Result<string> { IsSuccessful = true, CustomData = suffix });
        }

        private void SetupSuggestUsernameFailure(int errorCode, string errorMessage)
        {
            _mockCustomerService.Setup(x => x.GetSuggestUsername(It.IsAny<GetSuggestUsernameRequest>()))
                .ReturnsAsync(new Result<string> { IsSuccessful = false, Code = errorCode, Message = errorMessage });
        }

        private void VerifyValidationCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<ValidateCashMemberSetting>()), Times.Once);
        }

        private void VerifyRegistrationCalled()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<RegisterCustomerBeforeAddNew>()), Times.Once);
        }

        private void VerifyRegistrationCalledMultipleTimes()
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<RegisterCustomerBeforeAddNew>()), Times.AtLeast(2));
        }

        private void VerifyRollbackCalled(int custId)
        {
            _mockDynamicRepository.Verify(x => x.ExecuteAsync(It.IsAny<RollbackRegisterCustomer>()), Times.Once);
        }
    }
}
