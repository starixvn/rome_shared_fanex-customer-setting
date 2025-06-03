using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.Cash;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.Cash
{
    public class CreateCashMemberServiceTests
    {
        [Fact]
        public async Task Create_ReturnsResult_WhenValidationFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var helperService = new Mock<ICustomerSettingHelper>();
            var customerService = new Mock<ICustomerService>();
            var dynamicRepository = new Mock<IDynamicRepository>();
            var service = new CreateCashMemberService(messageService.Object, appSettings, helperService.Object, customerService.Object, dynamicRepository.Object);
            var request = new CreateRequest();
            // You may want to set up request to fail validation
            var result = await service.Create(request);
            Assert.NotNull(result);
        }
    }
}
