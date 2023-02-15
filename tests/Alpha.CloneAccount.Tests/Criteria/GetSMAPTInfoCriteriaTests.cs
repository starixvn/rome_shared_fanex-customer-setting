namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class GetSMAPTInfoCriteriaTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var criteria = new GetSMAPTInfoCriteria(UserLevel.Super, 33120484);

            // Assert
            Assert.True(
                criteria.CustLevel == UserLevel.Super
                && criteria.CustId == 33120484);
        }

        [Theory]
        [InlineData(2, "Acc_CustomerSelAgentInfo")]
        [InlineData(3, "Acc_CustomerSelMAgentInfo")]
        [InlineData(4, "Acc_CustomerSelSAgentInfo")]
        public void GetSettingKey_WithCorrectCustLevel_ReturnsValidSettingKey(byte custLevelId, string expectedSettingKey)
        {
            // Arrange
            var custLevel = Enumeration.FromValue<UserLevel>(custLevelId);
            var criteria = new GetSMAPTInfoCriteria(custLevel, 33120484);

            // Act & Assert
            Assert.Equal(expectedSettingKey, criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120484, true)]
        public void IsValid_CustId_ReturnsExpectedResult(int custId, bool expectedResult)
        {
            var criteria = new GetSMAPTInfoCriteria(UserLevel.Master, custId);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
