using System.Text.Json;
using System.Threading.Tasks;
using Fanex.Customer.API.Modules.CreateCustomer.EndPoints;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.CreateCustomer.EndPoints
{
    public class CreateCashCustomerEndpointTests
    {
        private readonly Mock<ICreateCustomerService> _serviceMock;
        private readonly CreateCashCustomerEndpoint _endpoint;
        private readonly CreateRequest _request;

        public CreateCashCustomerEndpointTests()
        {
            _serviceMock = new Mock<ICreateCustomerService>();
            _endpoint = new CreateCashCustomerEndpoint();
            _request = new CreateRequest
            {
                Context = new Fanex.Customer.API.Modules.CreateCustomer.Requests.RequestContext()
            };
        }

        [Fact]
        public async Task HandleAsync_InvalidRequest_ReturnsErrorResult()
        {
            _serviceMock.Setup(s => s.ValidateRequest(_request)).Returns(false);

            var result = await _endpoint.HandleAsync(_request, _serviceMock.Object);

            var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<object>>(result);
            var error = Assert.IsType<Result>(okResult.Value);
            Assert.False(error.IsSuccessful);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsCreateCustomerResult()
        {
            _serviceMock.Setup(s => s.ValidateRequest(_request)).Returns(true);
            var expected = new CreateCustomerResult(1, 0, string.Empty, null);
            _serviceMock.Setup(s => s.Create(_request)).ReturnsAsync(expected);

            var result = await _endpoint.HandleAsync(_request, _serviceMock.Object);

            var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<object>>(result);
            Assert.Equal(expected, okResult.Value);
        }
    }
}
