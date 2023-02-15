using Alpha.CloneAccount.Criteria;
using Xunit;

namespace Alpha.CloneAccount.Tests.Criteria
{
    public class CheckLimitNewDownlineCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new CheckLimitNewDownlineCommand(1);
            var expectedResult = "Acc_ProblemAccountLimitDownlineCheck";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void IsValid_Always_ReturnsExpectedResult(int custId, bool expected)
        {
            // Arrange
            var criteria = new CheckLimitNewDownlineCommand(custId);

            // Act & Assert
            Assert.Equal(expected, criteria.IsValid());
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void IsAllowed_Always_AssignsCorrectValue(bool isAllowed, bool expected)
        {
            // Arrange & Act
            var criteria = new CheckLimitNewDownlineCommand(1) { IsAllowed = isAllowed };

            // Assert
            Assert.Equal(expected, criteria.IsAllowed);
        }
    }
}
