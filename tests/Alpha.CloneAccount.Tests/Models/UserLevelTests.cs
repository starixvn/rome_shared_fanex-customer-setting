namespace Alpha.CloneAccount.Tests.Models
{
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using Xunit;

    [Trait("Category", "Models")]
    public class UserLevelTests
    {
        [Theory]
        [InlineData(4, false)]
        [InlineData(3, false)]
        [InlineData(2, true)]
        [InlineData(1, false)]
        public void IsAgentLevel_Always_ReturnsExpectedResult(byte roleId, bool expectedResult)
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(roleId);

            // Act
            var result = userLevel.IsAgentLevel();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(4, false)]
        [InlineData(3, true)]
        [InlineData(2, false)]
        [InlineData(1, false)]
        public void IsMasterLevel_Always_ReturnsExpectedResult(byte roleId, bool expectedResult)
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(roleId);

            // Act
            var result = userLevel.IsMasterLevel();

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
