namespace Alpha.CloneAccount.Tests.Models
{
    using Xunit;
    using Result = CloneAccount.Models.Result;

    [Trait("Category", "Models")]
    public class ResultTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var actualResult = new Result(0, string.Empty);

            // Assert
            Assert.True(
                actualResult.Code == 0
                && actualResult.Message == string.Empty
                && actualResult.IsSuccess);
        }

        [Fact]
        public void IsSuccess_ValidCode_ReturnsTrue()
        {
            // Arrange
            var result = new Result(0, string.Empty);

            // Act
            var actualResult = result.IsSuccess;

            // Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void IsSuccess_InvalidCode_ReturnsFalse()
        {
            // Arrange
            var result = new Result(201, "Master Max Credit must be between 0 and 24,875,698");

            // Act
            var actualResult = result.IsSuccess;

            // Assert
            Assert.False(actualResult);
        }
    }
}
