namespace Alpha.CloneAccount.Api.Tests.Requests
{
    using Api.Requests;
    using Xunit;

    [Trait("Category", "Requests")]
    public class ValidateMaxNewMemberQueryTests
    {
        [Fact]
        public void ValidateMaxNewMemberQuery_DefaultConstructor_AssignsCorrectDefaultValue()
        {
            // Arrange & Act
            var request = new ValidateMaxNewMemberQuery(8, "en-US");

            // Assert
            Assert.True(
                request.AgentId == 8
                && request.Language == "en-US");
        }

        [Theory]
        [InlineData(-8, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_AgentId_ReturnsCorrectExpectedResult(int agentId, bool expected)
        {
            // Arrange
            var request = new ValidateMaxNewMemberQuery(agentId, "en-US");

            // Act
            var actual = request.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
