namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Constants;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class CopyAgentCommandTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectMaxCredit()
        {
            // Arrange & Act
            var criteria = GetCopyAgentCommand(8);

            // Assert
            Assert.True(criteria.MaxCredit == 1000000);
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = GetCopyAgentCommand(8);
            var expectedResult = "Acc_CustomerInsMember_CloneSetting_MAg";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_MRecommend_ReturnsExpectedResult(int mrecommend, bool expectedResult)
        {
            var criteria = GetCopyAgentCommand(mrecommend);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        private static CopyAgentCommand GetCopyAgentCommand(int creator)
            => new CopyAgentCommand(
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
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
