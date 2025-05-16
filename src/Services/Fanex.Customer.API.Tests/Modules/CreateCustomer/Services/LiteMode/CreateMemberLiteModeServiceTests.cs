using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Services.LiteMode;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;
using Moq;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.LiteMode
{
    public class CreateMemberLiteModeServiceTests
    {
        [Fact]
        public async Task Create_ReturnsResult_WhenValidationFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var helperService = new Mock<ICustomerSettingHelper>();
            var dynamicRepository = new Mock<IDynamicRepository>();
            var service = new CreateMemberLiteModeService(messageService.Object, appSettings, helperService.Object, dynamicRepository.Object);
            var request = new CreateRequest();
            var result = await service.Create(request);
            Assert.NotNull(result);
        }
    }
}
