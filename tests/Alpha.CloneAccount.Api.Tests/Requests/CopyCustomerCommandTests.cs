namespace Alpha.CloneAccount.Api.Tests.Requests
{
    using Api.Requests;
    using Constants;
    using Xunit;

    [Trait("Category", "Requests")]
    public class CopyCustomerCommandTests
    {
        [Fact]
        public void CopyCustomerCommand_DefaultConstructor_AssignsCorrectDefaultValue()
        {
            // Arrange & Act
            var command = new CopyCustomerCommand();

            // Assert
            Assert.True(
                command.ActorId == 0
                && command.ActorSubAccId == 0
                && command.ActorSubAccName == null
                && command.ActorUserLevel == 0
                && command.UserName == null
                && command.Password == null
                && command.FirstName == null
                && command.LastName == null
                && command.Phone == null
                && command.MobilePhone == null
                && command.Fax == null
                && command.Credit == 0
                && command.MaxCredit == 0
                && command.SourceCustId == 0
                && command.Language == null
                && command.IP == null
                && command.SystemId == 0);
        }

        [Theory]
        [InlineData(-8, false)]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_ActorId_ReturnsCorrectExpectedResult(int actorId, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = actorId,
                ActorSubAccId = 3987,
                ActorUserLevel = 4,
                UserName = "super",
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-3987, false)]
        [InlineData(0, true)]
        [InlineData(3987, true)]
        public void IsValid_ActorSubAccId_ReturnsCorrectExpectedResult(int actorSubAccId, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = actorSubAccId,
                ActorUserLevel = 4,
                UserName = "super",
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(8, false)]
        [InlineData(2, true)]
        public void IsValid_ActorUserLevel_ReturnsCorrectExpectedResult(byte actorUserLevel, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = actorUserLevel,
                UserName = "super",
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("     ", false)]
        [InlineData("super", true)]
        public void IsValid_UserName_ReturnsCorrectExpectedResult(string userName, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = 4,
                UserName = userName,
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-100, false)]
        [InlineData(0, true)]
        [InlineData(100, true)]
        public void IsValid_Credit_ReturnsCorrectExpectedResult(int credit, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = 4,
                UserName = "super",
                Credit = credit,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-100, false)]
        [InlineData(0, true)]
        [InlineData(100, true)]
        public void IsValid_MaxCredit_ReturnsCorrectExpectedResult(int maxCredit, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = 4,
                UserName = "super",
                Credit = 100,
                MaxCredit = maxCredit,
                SourceCustId = 3985,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-3985, false)]
        [InlineData(0, false)]
        [InlineData(3985, true)]
        public void IsValid_SourceCustId_ReturnsCorrectExpectedResult(int sourceCustId, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = 4,
                UserName = "super",
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = sourceCustId,
                SystemId = SystemIdentity.Chatbot
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(8, true)]
        public void IsValid_SystemId_ReturnsCorrectExpectedResult(byte systemId, bool expected)
        {
            // Arrange
            var command = new CopyCustomerCommand
            {
                ActorId = 8,
                ActorSubAccId = 3987,
                ActorUserLevel = 4,
                UserName = "super",
                Credit = 100,
                MaxCredit = 1000,
                SourceCustId = 3985,
                SystemId = systemId
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
