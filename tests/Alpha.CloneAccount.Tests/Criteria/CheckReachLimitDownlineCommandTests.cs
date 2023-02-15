using Alpha.CloneAccount.Criteria;
using Xunit;

namespace Alpha.CloneAccount.Tests.Criteria
{
    public class CheckReachLimitDownlineCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new CheckReachLimitDownlineCommand(1, 2, "username", "E");
            var expectedResult = "Age_CustomerCheckReachDownline";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_InvalidCustId_ReturnFalse(int custId)
        {
            // Arrange
            var criteria = new CheckReachLimitDownlineCommand(custId, 2, "username", "E");

            // Act & Assert
            Assert.False(criteria.IsValid());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void IsValid_InvalidRoleId_ReturnFalse(int roleId)
        {
            // Arrange
            var criteria = new CheckReachLimitDownlineCommand(1, roleId, "username", "E");

            // Act & Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCriteria_ReturnTrue()
        {
            // Arrange
            var criteria = new CheckReachLimitDownlineCommand(1, 2, "username", "E");

            // Act & Assert
            Assert.True(criteria.IsValid());
        }
    }
}