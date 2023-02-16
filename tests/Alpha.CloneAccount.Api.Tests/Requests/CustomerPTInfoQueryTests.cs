namespace Alpha.CloneAccount.Api.Tests.Requests
{
    using Api.Requests;
    using Xunit;

    [Trait("Category", "Requests")]
    public class CustomerPTInfoQueryTests
    {
        [Fact]
        public void CustomerPTInfoQuery_DefaultConstructor_AssignsCorrectDefaultValue()
        {
            // Arrange & Act
            var ptInfo = new CustomerPTInfoQuery(8, 4);

            // Assert
            Assert.True(
                ptInfo.CustId == 8
                && ptInfo.CustLevel == 4);
        }

        [Theory]
        [InlineData(-8, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_CustId_ReturnsCorrectExpectedResult(int custId, bool expected)
        {
            // Arrange
            var ptInfo = new CustomerPTInfoQuery(custId, 4);

            // Act
            var actual = ptInfo.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(8, false)]
        [InlineData(4, true)]
        public void IsValid_CustLevel_ReturnsCorrectExpectedResult(byte custLevel, bool expected)
        {
            // Arrange
            var ptInfo = new CustomerPTInfoQuery(8, custLevel);

            // Act
            var actual = ptInfo.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
