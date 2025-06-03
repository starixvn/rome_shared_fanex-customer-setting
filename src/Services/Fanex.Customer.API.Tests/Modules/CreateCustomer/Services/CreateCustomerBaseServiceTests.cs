using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;
using Moq;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services
{
    public class CreateCustomerBaseServiceTests
    {
        [Fact]
        public void Constructor_SetsDependencies()
        {
            var messageService = new Mock<IMessageService>().Object;
            var appSettings = new AppSettings();
            var helperService = new Mock<ICustomerSettingHelper>().Object;
            var dynamicRepository = new Mock<IDynamicRepository>().Object;

            var service = new CreateCustomerBaseService(messageService, appSettings, helperService, dynamicRepository);

            Assert.NotNull(service);
        }

        [Fact]
        public async Task Create_ThrowsNotImplementedException()
        {
            var service = new CreateCustomerBaseService(
                new Mock<IMessageService>().Object,
                new AppSettings(),
                new Mock<ICustomerSettingHelper>().Object,
                new Mock<IDynamicRepository>().Object);

            await Assert.ThrowsAsync<NotImplementedException>(() => service.Create(new CreateRequest()));
        }
    }
}
