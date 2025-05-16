using Fanex.Customer.API.Modules.CopyCustomer;
using Xunit;

namespace Fanex.Customer.API.Tests.Modules.CopyCustomer
{
    public class CopyCustomerRequestTests
    {
        [Fact]
        public void IsValid_ReturnsTrue_WhenAllRequiredFieldsAreValid()
        {
            var request = new CopyCustomerRequest
            {
                ActorId = 1,
                ActorSubAccId = 0,
                UplineId = 0,
                CustLevel = 2,
                UserName = "user",
                Password = "pass",
                Credit = 100,
                MaxCredit = 100,
                SourceCustId = 1,
                SystemId = 1
            };

            Assert.True(request.IsValid());
        }

        [Theory]
        [InlineData(0, 0, 0, 2, "user", "pass", 100, 100, 1, 1, false)] // ActorId = 0
        [InlineData(1, -1, 0, 2, "user", "pass", 100, 100, 1, 1, false)] // ActorSubAccId < 0
        [InlineData(1, 0, -1, 2, "user", "pass", 100, 100, 1, 1, false)] // UplineId < 0
        [InlineData(1, 0, 0, 0, "user", "pass", 100, 100, 1, 1, false)] // CustLevel < 1
        [InlineData(1, 0, 0, 5, "user", "pass", 100, 100, 1, 1, false)] // CustLevel >= 5
        [InlineData(1, 0, 0, 2, null, "pass", 100, 100, 1, 1, false)] // UserName null
        [InlineData(1, 0, 0, 2, "", "pass", 100, 100, 1, 1, false)] // UserName empty
        [InlineData(1, 0, 0, 2, "user", null, 100, 100, 1, 1, false)] // Password null
        [InlineData(1, 0, 0, 2, "user", "", 100, 100, 1, 1, false)] // Password empty
        [InlineData(1, 0, 0, 2, "user", "pass", -1, 100, 1, 1, false)] // Credit < 0
        [InlineData(1, 0, 0, 2, "user", "pass", 100, -1, 1, 1, false)] // MaxCredit < 0
        [InlineData(1, 0, 0, 2, "user", "pass", 100, 100, 0, 1, false)] // SourceCustId <= 0
        [InlineData(1, 0, 0, 2, "user", "pass", 100, 100, 1, 0, false)] // SystemId <= 0
        public void IsValid_VariousCases_ReturnsExpected(
            int actorId, int actorSubAccId, int uplineId, byte custLevel, string userName, string password,
            decimal credit, decimal maxCredit, int sourceCustId, byte systemId, bool expected)
        {
            var request = new CopyCustomerRequest
            {
                ActorId = actorId,
                ActorSubAccId = actorSubAccId,
                UplineId = uplineId,
                CustLevel = custLevel,
                UserName = userName,
                Password = password,
                Credit = credit,
                MaxCredit = maxCredit,
                SourceCustId = sourceCustId,
                SystemId = systemId
            };

            Assert.Equal(expected, request.IsValid());
        }
    }
}
