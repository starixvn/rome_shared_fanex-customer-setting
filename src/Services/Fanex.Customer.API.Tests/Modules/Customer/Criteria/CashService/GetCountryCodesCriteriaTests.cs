using Xunit;
using Fanex.Customer.API.Modules.Customer.Criteria.CashService;

namespace Fanex.Customer.API.Tests.Modules.Customer.Criteria.CashService;

public class GetCountryCodesCriteriaTests
{
    [Fact]
    public void Constructor_WithSite_SetsSiteProperty()
    {
        // Arrange
        var site = "test-site";

        // Act
        var criteria = new GetCountryCodesCriteria(site);

        // Assert
        Assert.Equal(site, criteria.Site);
        Assert.Equal("Age_CashService_CountryCodeSel", criteria.GetSettingKey());
    }

    [Theory]
    [InlineData("US", true)]
    [InlineData("CA", true)]
    [InlineData("test-site", true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(null, false)]
    public void IsValid_WithDifferentSites_ReturnsExpectedResult(string site, bool expectedResult)
    {
        // Act
        var criteria = new GetCountryCodesCriteria(site);

        // Assert
        Assert.Equal(expectedResult, criteria.IsValid());
    }

    [Fact]
    public void GetSettingKey_ReturnsCorrectStoredProcedureName()
    {
        // Arrange
        var site = "any-site";
        var criteria = new GetCountryCodesCriteria(site);

        // Act
        var settingKey = criteria.GetSettingKey();

        // Assert
        Assert.Equal("Age_CashService_CountryCodeSel", settingKey);
    }
}
