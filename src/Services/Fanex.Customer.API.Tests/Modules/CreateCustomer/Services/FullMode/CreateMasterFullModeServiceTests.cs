using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;
using Moq;
using Xunit;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using System.Collections.Generic;

namespace Fanex.Customer.API.Tests.Modules.CreateCustomer.Services.FullMode
{
    public class CreateMasterFullModeServiceTests
    {
        [Fact]
        public async Task Create_ReturnsResult_WhenValidationFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var helperService = new Mock<ICustomerSettingHelper>();
            var dynamicRepository = new Mock<IDynamicRepository>();
            var service = new CreateMasterFullModeService(messageService.Object, appSettings, helperService.Object, dynamicRepository.Object);
            var request = new CreateRequest
            {
                Context = new RequestContext(),
                Settings = new CustomerSettingModel
                {
                    GeneralInfo = new GeneralInformation(),
                    ProductGroups = new List<ProductGroup>()
                }
            };
            var result = await service.Create(request);
            Assert.NotNull(result);
        }
    }
}
