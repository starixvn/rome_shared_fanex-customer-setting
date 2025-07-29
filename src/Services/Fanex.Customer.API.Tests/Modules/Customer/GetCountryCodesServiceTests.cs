using Xunit;
using Moq;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Criteria.CashService;
using Fanex.Customer.API.Shared.Services;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Tests.Modules.Customer;

public class GetCountryCodesServiceTests
{
    private readonly Mock<IDynamicRepository> mockRepository;
    private readonly Mock<ICacheRefService> mockCacheRefService;
    private readonly Mock<ISiteDBService> mockSiteDBService;
    private readonly CustomerService customerService;

    public GetCountryCodesServiceTests()
    {
        mockRepository = new Mock<IDynamicRepository>();
        mockCacheRefService = new Mock<ICacheRefService>();
        mockSiteDBService = new Mock<ISiteDBService>();
        
        var mockAppSettings = new Mock<IOptions<AppSettings>>();
        mockAppSettings.Setup(x => x.Value).Returns(new AppSettings());
        
        customerService = new CustomerService(
            mockRepository.Object, 
            mockCacheRefService.Object, 
            mockSiteDBService.Object, 
            mockAppSettings.Object);
    }

    [Fact]
    public async Task GetCountryCodes_WithValidSite_ReturnsCountryCodes()
    {
        // Arrange
        var site = "test-site";
        var expectedCountryCodes = new List<string> { "US", "CA", "GB", "AU" };
        
        mockRepository.Setup(x => x.FetchAsync<string>(It.IsAny<GetCountryCodesCriteria>()))
                     .ReturnsAsync(expectedCountryCodes);

        // Act
        var result = await customerService.GetCountryCodes(site);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCountryCodes.Count, result.Count());
        Assert.Equal(expectedCountryCodes, result);
        
        mockRepository.Verify(x => x.FetchAsync<string>(It.Is<GetCountryCodesCriteria>(c => c.Site == site)), Times.Once);
    }

    [Fact]
    public async Task GetCountryCodes_WithEmptyResult_ReturnsEmptyCollection()
    {
        // Arrange
        var site = "empty-site";
        var expectedCountryCodes = new List<string>();
        
        mockRepository.Setup(x => x.FetchAsync<string>(It.IsAny<GetCountryCodesCriteria>()))
                     .ReturnsAsync(expectedCountryCodes);

        // Act
        var result = await customerService.GetCountryCodes(site);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public async Task GetCountryCodes_WithWhitespaceSite_StillCallsRepository(string site)
    {
        // Arrange
        var expectedCountryCodes = new List<string>();
        
        mockRepository.Setup(x => x.FetchAsync<string>(It.IsAny<GetCountryCodesCriteria>()))
                     .ReturnsAsync(expectedCountryCodes);

        // Act
        var result = await customerService.GetCountryCodes(site);

        // Assert
        Assert.NotNull(result);
        mockRepository.Verify(x => x.FetchAsync<string>(It.Is<GetCountryCodesCriteria>(c => c.Site == site)), Times.Once);
    }
}
