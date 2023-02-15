namespace Alpha.CloneAccount.Tests.Models
{
    using CloneAccount.Models;
    using Xunit;

    [Trait("Category", "Models")]
    public class CustomerPTInfoTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var actualResult = new CustomerPTInfo(33120484, 3, "00", 1000000);

            // Assert
            Assert.True(
                actualResult.CustId == 33120484
                && actualResult.CustLevel == 3
                && actualResult.Suffix == "00"
                && actualResult.MaxCredit == 1000000);
        }
    }
}
