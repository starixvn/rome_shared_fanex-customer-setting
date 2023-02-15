namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Constants;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class CopyMasterCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = GetCopyMasterCommand(8);
            var expectedResult = "Acc_CustomerInsMember_CloneSetting_SAg";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_SRecommend_ReturnsExpectedResult(int srecommend, bool expectedResult)
        {
            var criteria = GetCopyMasterCommand(srecommend);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        private static CopyMasterCommand GetCopyMasterCommand(int creator)
            => new CopyMasterCommand(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                creator,
                creator,
                33120485,
                0,
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                1);
    }
}
