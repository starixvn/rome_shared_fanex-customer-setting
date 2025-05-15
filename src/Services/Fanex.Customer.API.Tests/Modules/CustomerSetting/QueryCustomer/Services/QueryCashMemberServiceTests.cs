using System.Collections.Generic;
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

namespace Fanex.Customer.API.Modules.CustomerSetting.QueryCustomer.Services.Tests
{
    public class QueryCashMemberServiceTests
    {
        private readonly Mock<ICustomerService> _customerService = new();
        private readonly Mock<ISiteDBGroupService> _siteDBGroupService = new();
        private readonly Mock<IDynamicRepository> _dynamicRepository = new();
        private readonly Mock<IOptions<AppSettings>> _appSettings = new();
        private readonly Mock<ISettingProfileService> _settingProfileService = new();
        private readonly Mock<ICacheRefService> _cacheRefService = new();
        private readonly Mock<IMessageService> _messageService = new();
        private readonly Mock<ICustomerSettingHelper> _helperService = new();

        private readonly AppSettings _settings = new AppSettings();

        private QueryCashCustomerService CreateService()
        {
            _appSettings.Setup(x => x.Value).Returns(_settings);
            return new QueryCashCustomerService(
                _customerService.Object,
                _siteDBGroupService.Object,
                _dynamicRepository.Object,
                _appSettings.Object,
                _settingProfileService.Object,
                _cacheRefService.Object,
                _messageService.Object,
                _helperService.Object);
        }

        [Fact]
        public async Task Query_ReturnsCustomerSettingModel_WithGeneralInfoAndProductGroups()
        {
            // Arrange
            var request = new QueryCashCustomerRequest
            {
                CustId = 1,
                SystemId = 1,
                RoleId = CustomerLevel.Member,
                UplineRoleId = CustomerLevel.Agent,
                IsGetUpline = false,
                ProductGroupId = null,
                Language = "en"
            };
            var generalInfoDto = new GeneralInformationDTO { CustId = 1, UserName = "test", Site = "site", SiteLabel = "label", Currency = 1 };
            _dynamicRepository.Setup(x => x.GetAsync<GeneralInformationDTO>(It.IsAny<ICriteria>())).ReturnsAsync(generalInfoDto);
            _settingProfileService.Setup(x => x.GetSettingProfiles(It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SettingProfileService.ProfileMode>(), It.IsAny<SettingProfileService.ServiceType>()))
                .ReturnsAsync(new List<SettingProfile>());
            var service = CreateService();

            // Act
            var result = await service.Query(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.GeneralInfo);
            Assert.Equal(generalInfoDto.CustId, result.GeneralInfo.CustId);
            Assert.NotNull(result.ProductGroups);
        }
    }
}
