using System.Threading.Tasks;
using Xunit;
using Moq;
using Fanex.Customer.API.Modules.CopyCustomer.Services;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Modules.CopyCustomer;

namespace Fanex.Customer.API.Tests.Modules.CopyCustomer
{
    public class CopySuperServiceTests
    {
        [Fact]
        public async Task Copy_ReturnsValidationFailure_WhenValidationFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var service = new TestableCopySuperService(messageService.Object, appSettings)
            {
                ValidateResult = new CopyCustomerResult(0, 1, 0, "Validation failed")
            };
            var request = new CopyCustomerRequest { UserName = "user", Site = "site", Credit = 100, SourceCustId = 1, ActorId = 1, Language = "en" };
            var result = await service.Copy(request);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Validation failed", result.MessageContent?.MainMessage);
        }

        [Fact]
        public async Task Copy_ReturnsRegisterFailure_WhenRegisterFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var service = new TestableCopySuperService(messageService.Object, appSettings)
            {
                ValidateResult = new CopyCustomerResult(0, 0, 0, "OK"),
                RegisterResult = new CopyCustomerResult(0, 2, 0, "Register failed")
            };
            var request = new CopyCustomerRequest { UserName = "user", Site = "site", Credit = 100, SourceCustId = 1, ActorId = 1, Language = "en" };
            var result = await service.Copy(request);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Register failed", result.MessageContent?.MainMessage);
        }

        [Fact]
        public async Task Copy_ReturnsCopyFailure_WhenCopySuperFails()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var service = new TestableCopySuperService(messageService.Object, appSettings)
            {
                ValidateResult = new CopyCustomerResult(0, 0, 0, "OK"),
                RegisterResult = new CopyCustomerResult(123, 0, 0, "OK"),
                CopySuperResult = new CopyCustomerResult(123, 3, 0, "Copy failed")
            };
            var request = new CopyCustomerRequest { UserName = "user", Site = "site", Credit = 100, SourceCustId = 1, ActorId = 1, Language = "en" };
            var result = await service.Copy(request);
            Assert.False(result.IsSuccessful);
            Assert.Equal("Copy failed", result.MessageContent?.MainMessage);
        }

        [Fact]
        public async Task Copy_ReturnsSuccess_WhenAllStepsSucceed()
        {
            var messageService = new Mock<IMessageService>();
            var appSettings = new AppSettings();
            var service = new TestableCopySuperService(messageService.Object, appSettings)
            {
                ValidateResult = new CopyCustomerResult(0, 0, 0, "OK"),
                RegisterResult = new CopyCustomerResult(123, 0, 0, "OK"),
                CopySuperResult = new CopyCustomerResult(123, 0, 0, "Success")
            };
            var request = new CopyCustomerRequest { UserName = "user", Site = "site", Credit = 100, SourceCustId = 1, ActorId = 1, Language = "en" };
            var result = await service.Copy(request);
            Assert.True(result.IsSuccessful);
            Assert.Equal("Success", result.MessageContent?.MainMessage);
        }

        private class TestableCopySuperService : CopySuperService
        {
            public CopyCustomerResult ValidateResult { get; set; }
            public CopyCustomerResult RegisterResult { get; set; }
            public CopyCustomerResult CopySuperResult { get; set; }

            public TestableCopySuperService(IMessageService messageService, AppSettings appSettings)
                : base(messageService, appSettings) { }

            public new async Task<CopyCustomerResult> Copy(CopyCustomerRequest request)
                => await base.Copy(request);
        }
    }
}
