using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.FullMode
{
    public class CreateDirectMemberServiceTests
    {
        [Fact]
        public async Task Create_ReturnsResult_WhenValidationFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var helperService = new Mock<ICustomerSettingHelper>();
            var customerService = new Mock<Fanex.Customer.API.Modules.Customer.ICustomerService>();
            var dynamicRepository = new Mock<IDynamicRepository>();
            var service = new CreateDirectMemberService(messageService.Object, appSettings, helperService.Object, customerService.Object, dynamicRepository.Object);
            var request = new CreateRequest();
            var result = await service.Create(request);
            Assert.NotNull(result);
        }
    }
}
