namespace Alpha.CloneAccount.Tests.Criteria
{
    using CloneAccount.Criteria;
    using Xunit;

    [Trait("Category", "Criteria")]
    public class InsertMemberDCSSettingCommandTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectErrCodeAndErrMsg()
        {
            // Arrange & Act
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, 0, string.Empty, 1)
            {
                ErrCode = 201,
                ErrMsg = "Master Max Credit must be between 0 and 24,875,698"
            };

            // Assert
            Assert.True(
                criteria.ErrCode == 201
                && criteria.ErrMsg == "Master Max Credit must be between 0 and 24,875,698");
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsValidSettingKey()
        {
            // Arrange
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, 0, string.Empty, 1);
            var expectedResult = "Acc_CustSetting_DCSSettingCopyInsertMember";

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120519, true)]
        public void IsValid_CustId_ReturnsExpectedResult(int custId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(custId, 33120520, 33120484, 0, string.Empty, 1);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120520, true)]
        public void IsValid_SourceCustId_ReturnsExpectedResult(int sourceCustId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(33120519, sourceCustId, 33120484, 0, string.Empty, 1);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, false)]
        [InlineData(33120484, true)]
        public void IsValid_ActorId_ReturnsExpectedResult(int actorId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, actorId, 0, string.Empty, 1);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(0, true)]
        [InlineData(int.MaxValue, true)]
        public void IsValid_ActorSubAccId_ReturnsExpectedResult(int actorSubAccId, bool expectedResult)
        {
            var criteria = new InsertMemberDCSSettingCommand(33120519, 33120520, 33120484, actorSubAccId, string.Empty, 1);

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
