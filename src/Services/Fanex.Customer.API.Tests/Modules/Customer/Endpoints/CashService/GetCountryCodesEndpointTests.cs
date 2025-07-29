using Xunit;
using Moq;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Endpoints.CashService;
using Microsoft.AspNetCore.Http;

namespace Fanex.Customer.API.Tests.Modules.Customer.Endpoints.CashService;

public class GetCountryCodesEndpointTests
{
    private readonly Mock<ICustomerService> mockCustomerService;
    private readonly GetCountryCodesEndpoint endpoint;

    public GetCountryCodesEndpointTests()
    {
        mockCustomerService = new Mock<ICustomerService>();
        endpoint = new GetCountryCodesEndpoint();
    }

    [Fact]
    public async Task HandleAsync_WithValidSite_ReturnsOkWithCountryCodes()
    {
        // Arrange
        var site = "test-site";
        var expectedCountryCodes = new List<string> { "US", "CA", "GB" };
        
        mockCustomerService.Setup(x => x.GetCountryCodes(site))
                          .ReturnsAsync(expectedCountryCodes);

        // Act
        var result = await endpoint.HandleAsync(site, mockCustomerService.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<IEnumerable<string>>>(result);
        mockCustomerService.Verify(x => x.GetCountryCodes(site), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public async Task HandleAsync_WithEmptyOrWhitespaceSite_ReturnsBadRequest(string site)
    {
        // Act
        var result = await endpoint.HandleAsync(site, mockCustomerService.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>(result);
        mockCustomerService.Verify(x => x.GetCountryCodes(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithNullSite_ReturnsBadRequest()
    {
        // Act
        var result = await endpoint.HandleAsync(null!, mockCustomerService.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>(result);
        mockCustomerService.Verify(x => x.GetCountryCodes(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ServiceReturnsEmptyList_ReturnsOkWithEmptyList()
    {
        // Arrange
        var site = "empty-site";
        var expectedCountryCodes = new List<string>();
        
        mockCustomerService.Setup(x => x.GetCountryCodes(site))
                          .ReturnsAsync(expectedCountryCodes);

        // Act
        var result = await endpoint.HandleAsync(site, mockCustomerService.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<IEnumerable<string>>>(result);
        mockCustomerService.Verify(x => x.GetCountryCodes(site), Times.Once);
    }
}
