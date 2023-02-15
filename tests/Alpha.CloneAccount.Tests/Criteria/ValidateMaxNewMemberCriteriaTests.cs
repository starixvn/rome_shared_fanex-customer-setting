namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class ValidateMaxNewMemberCriteriaTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var criteria = new ValidateMaxNewMemberCriteria(33120484, "en-US");

            // Assert
            Assert.True(
                criteria.AgentId == 33120484
                && criteria.Language == "en-US");
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new ValidateMaxNewMemberCriteria(33120484, "en-US");
            var expectedResult = "Age_MaxMemberLimitBySuperCheck";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120484, true)]
        public void IsValid_AgentId_ReturnsExpectedResult(int agentId, bool expectedResult)
        {
            var criteria = new ValidateMaxNewMemberCriteria(agentId, "en-US");

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
