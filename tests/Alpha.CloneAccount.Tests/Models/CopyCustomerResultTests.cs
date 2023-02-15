namespace Alpha.CloneAccount.Tests.Models
{
    using CloneAccount.Models;
    using Xunit;

    [Trait("Category", "Models")]
    public class CopyCustomerResultTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var actualResult = new CopyCustomerResult(33120484, "agent", "1234aa", 2, "sourceUsername", 0, 0, 0, string.Empty);

            // Assert
            Assert.True(
                actualResult.CustId == 33120484
                && actualResult.CustName == "agent"
                && actualResult.Password == "1234aa"
                && actualResult.CustLevel == 2
                && actualResult.SourceUsername == "sourceUsername"
                && actualResult.Credit == 0
                && actualResult.MaxCredit == 0
                && actualResult.Code == 0
                && actualResult.Message == string.Empty
                && actualResult.IsSuccess);
        }

        [Fact]
        public void IsSuccess_ValidCode_ReturnsTrue()
        {
            // Arrange
            var result = new CopyCustomerResult(33120484, "agent", "1234aa", 2, "sourceUsername", 0, 0, 0, string.Empty);

            // Act
            var actualResult = result.IsSuccess;

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void IsSuccess_InvalidCode_ReturnsFalse()
        {
            // Arrange
            var result = new CopyCustomerResult(0, string.Empty, string.Empty, 0, string.Empty, 0, 0, 201, "Master Max Credit must be between 0 and 24,875,698");

            // Act
            var actualResult = result.IsSuccess;

            // Assert
            Assert.False(actualResult);
        }
    }
}