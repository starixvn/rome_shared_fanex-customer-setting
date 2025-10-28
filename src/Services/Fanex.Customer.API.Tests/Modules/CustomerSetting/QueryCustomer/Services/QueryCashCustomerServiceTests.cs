using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Modules.QueryCustomer.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.AfterQuery;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.EditSettings;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria;
using static Fanex.Customer.API.Modules.CustomerSetting.Shared.Services.SettingProfileService;

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.Tests
{
    /// <summary>
    /// Unit tests for QueryCashCustomerService
    /// Tests the main functionality including Query method with various scenarios
    /// </summary>
    public class QueryCashCustomerServiceTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock = new();
        private readonly Mock<ISiteDBGroupService> _siteDBGroupServiceMock = new();
        private readonly Mock<IDynamicRepository> _dynamicRepositoryMock = new();
        private readonly Mock<IOptions<AppSettings>> _appSettingsMock = new();
        private readonly Mock<ISettingProfileService> _settingProfileServiceMock = new();
        private readonly Mock<ICacheRefService> _cacheRefServiceMock = new();
        private readonly Mock<IMessageService> _messageServiceMock = new();
        private readonly Mock<ICustomerSettingHelper> _helperServiceMock = new();
        private readonly Mock<IAfterQueryService> _afterQueryServiceMock = new();

        private readonly AppSettings _appSettings = new AppSettings();

        private QueryCashCustomerService CreateService()
        {
            _appSettingsMock.Setup(x => x.Value).Returns(_appSettings);
            
            return new QueryCashCustomerService(
                _customerServiceMock.Object,
                _siteDBGroupServiceMock.Object,
                _dynamicRepositoryMock.Object,
                _appSettingsMock.Object,
                _settingProfileServiceMock.Object,
                _cacheRefServiceMock.Object,
                _messageServiceMock.Object,
                _helperServiceMock.Object,
                _afterQueryServiceMock.Object
            );
        }

        [Fact]
        public async Task Query_WithValidRequest_ReturnsCustomerSettingModel()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                UserName = "testuser",
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1,
                RoleId = CustomerLevel.Member
            };

            var settingProfiles = new List<SettingProfile>
            {
                new SettingProfile
                {
                    GroupId = 150,
                    GroupType = 150,
                    DisplayMode = 1,
                    DisplayType = 1
                }
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    It.IsAny<ServiceType>()))
                .ReturnsAsync(settingProfiles);

            _cacheRefServiceMock
                .Setup(x => x.GetProductGroupName(It.IsAny<int>(), It.IsAny<string>()))
                .Returns("Test Product Group");

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.GeneralInfo);
            Assert.Equal(123, result.GeneralInfo.CustId);
            Assert.Equal("testuser", result.GeneralInfo.UserName);
            Assert.Equal(CustomerLevel.Member, result.GeneralInfo.UplineRoleId);
            Assert.NotNull(result.ProductGroups);
        }

        [Fact]
        public async Task Query_WithUplineRequest_UsesCorrectCriteria()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Agent,
                Language = "en",
                IsGetUpline = true
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                UserName = "testuser",
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1,
                RoleId = CustomerLevel.Agent
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetGeneralInfoForEditCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    ProfileMode.Edit,
                    ServiceType.Cash))
                .ReturnsAsync(new List<SettingProfile>());

            // Act
            await service.Query(request);

            // Assert
            _dynamicRepositoryMock.Verify(
                x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetGeneralInfoForEditCriteria>()),
                Times.Once);

            _settingProfileServiceMock.Verify(
                x => x.GetSettingProfiles(
                    (byte)request.SystemId,
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    ProfileMode.Edit,
                    ServiceType.Cash),
                Times.Once);
        }

        [Fact]
        public async Task Query_WithNewCustomerRequest_UsesAddNewProfileMode()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Agent,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    ProfileMode.AddNew,
                    ServiceType.Cash))
                .ReturnsAsync(new List<SettingProfile>());

            // Act
            await service.Query(request);

            // Assert
            _settingProfileServiceMock.Verify(
                x => x.GetSettingProfiles(
                    (byte)request.SystemId,
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    ProfileMode.AddNew,
                    ServiceType.Cash),
                Times.Once);
        }

        [Fact]
        public async Task Query_WithSpecificProductGroup_FiltersCorrectly()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                ProductGroupId = 150,
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1
            };

            var settingProfiles = new List<SettingProfile>
            {
                new SettingProfile { GroupId = 150, GroupType = 150, DisplayMode = 1 },
                new SettingProfile { GroupId = 151, GroupType = 151, DisplayMode = 1 }
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    It.IsAny<ServiceType>()))
                .ReturnsAsync(settingProfiles);

            _cacheRefServiceMock
                .Setup(x => x.GetProductGroupName(150, It.IsAny<string>()))
                .Returns("Product Group 150");

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ProductGroups);
            
            // Should only contain the requested product group
            var productGroups = result.ProductGroups.ToList();
            Assert.All(productGroups, pg => Assert.Equal(150, pg.Id));
        }

        [Fact]
        public async Task Query_WithHiddenGroupType_ExcludesFromResult()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1
            };

            var settingProfiles = new List<SettingProfile>
            {
                new SettingProfile 
                { 
                    GroupId = 150, 
                    GroupType = 150, 
                    DisplayMode = DisplayMode.Hidden // Hidden group type
                },
                new SettingProfile 
                { 
                    GroupId = 151, 
                    GroupType = 151, 
                    DisplayMode = 1 // Visible group type
                }
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    It.IsAny<ServiceType>()))
                .ReturnsAsync(settingProfiles);

            _cacheRefServiceMock
                .Setup(x => x.GetProductGroupName(151, It.IsAny<string>()))
                .Returns("Product Group 151");

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ProductGroups);
            
            // Should not contain hidden group type (150)
            var productGroups = result.ProductGroups.ToList();
            Assert.DoesNotContain(productGroups, pg => pg.Id == 150);
        }

        [Theory]
        [InlineData("777")]
        [InlineData("OTHER")]
        public async Task Query_ProcessesDifferentSiteLabels(string siteLabel)
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = siteLabel,
                Currency = 1
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    It.IsAny<ServiceType>()))
                .ReturnsAsync(new List<SettingProfile>());

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(siteLabel, result.GeneralInfo.SiteLabel);
        }

        [Fact]
        public async Task Query_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.Query(null));
        }

        [Fact]
        public async Task Query_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.Query(request));
        }

        [Fact]
        public async Task Query_ServiceType_IsCashService()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    ServiceType.Cash))
                .ReturnsAsync(new List<SettingProfile>());

            // Act
            await service.Query(request);

            // Assert
            _settingProfileServiceMock.Verify(
                x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    ServiceType.Cash),
                Times.Once);
        }

        [Fact]
        public async Task Query_CallsAllRequiredServices()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    It.IsAny<ServiceType>()))
                .ReturnsAsync(new List<SettingProfile>());

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.GeneralInfo);
            Assert.NotNull(result.ProductGroups);

            // Verify all services were called
            _dynamicRepositoryMock.Verify(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()), Times.Once);
            _settingProfileServiceMock.Verify(x => x.GetSettingProfiles(It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ProfileMode>(), It.IsAny<ServiceType>()), Times.Once);
        }

        [Fact]
        public async Task Query_WithEmptySettingProfiles_ReturnsEmptyProductGroups()
        {
            // Arrange
            var service = CreateService();
            var request = new QueryCashCustomerRequest
            {
                CustId = 123,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                Language = "en",
                IsGetUpline = false
            };

            var generalInfoDto = new GeneralInformationDTO
            {
                CustId = 123,
                Site = "test",
                SiteLabel = "TEST",
                Currency = 1
            };

            _dynamicRepositoryMock
                .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                .ReturnsAsync(generalInfoDto);

            _settingProfileServiceMock
                .Setup(x => x.GetSettingProfiles(
                    It.IsAny<byte>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<ProfileMode>(),
                    It.IsAny<ServiceType>()))
                .ReturnsAsync(new List<SettingProfile>());

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ProductGroups);
            Assert.Empty(result.ProductGroups);
        }

        [Fact]
        public async Task Query_WithDifferentCustomerLevels_ProcessesCorrectly()
        {
            // Arrange
            var service = CreateService();
            
            foreach (var roleId in new[] { CustomerLevel.Company, CustomerLevel.Super, CustomerLevel.Master, CustomerLevel.Agent, CustomerLevel.Member })
            {
                var request = new QueryCashCustomerRequest
                {
                    CustId = 123,
                    SystemId = 1,
                    RoleId = roleId,
                    Language = "en",
                    IsGetUpline = false
                };

                var generalInfoDto = new GeneralInformationDTO
                {
                    CustId = 123,
                    Site = "test",
                    SiteLabel = "TEST",
                    Currency = 1,
                    RoleId = roleId
                };

                _dynamicRepositoryMock
                    .Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<GetAgentCriteria>()))
                    .ReturnsAsync(generalInfoDto);

                _settingProfileServiceMock
                    .Setup(x => x.GetSettingProfiles(
                        It.IsAny<byte>(),
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<ProfileMode>(),
                        It.IsAny<ServiceType>()))
                    .ReturnsAsync(new List<SettingProfile>());

                // Act
                var result = await service.Query(request);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(roleId, result.GeneralInfo.UplineRoleId);
            }
        }
    }
}
