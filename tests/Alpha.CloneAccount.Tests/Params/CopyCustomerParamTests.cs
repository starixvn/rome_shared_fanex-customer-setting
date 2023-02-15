namespace Alpha.CloneAccount.Tests.Params
{
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using CloneAccount.Params;
    using Constants;
    using Xunit;

    [Trait("Category", "Params")]
    public class CopyCustomerParamTests
    {
        [Fact]
        public void Constructor_Always_AssignsCorrectProperties()
        {
            // Arrange & Act
            var actualResult = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                false);

            // Assert
            Assert.True(
                actualResult.ActorId == 33120484
                && actualResult.ActorSubAccId == 0
                && actualResult.ActorSubAccName == string.Empty
                && actualResult.UserName == "master"
                && actualResult.Password == "1234aa"
                && actualResult.FirstName == string.Empty
                && actualResult.LastName == string.Empty
                && actualResult.Phone == string.Empty
                && actualResult.MobilePhone == string.Empty
                && actualResult.Fax == string.Empty
                && actualResult.Credit == 1000
                && actualResult.MaxCredit == 1000000
                && actualResult.SourceCustId == 33120480
                && actualResult.SourceUsername == "sourceUsername"
                && actualResult.Language == "en-US"
                && actualResult.IP == string.Empty
                && actualResult.SystemId == SystemIdentity.Chatbot
                && actualResult.LanguageKey == "E");
        }

        [Fact]
        public void SetPassword_Always_AssignsCorrectPassword()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                false);

            // Act
            param.SetPassword("1234aaAA");

            // Assert
            Assert.Equal("1234aaAA", param.Password);
        }

        [Fact]
        public void SetUserName_Always_AssignsCorrectUserName()
        {
            // Arrange
            var param = new CopyCustomerParam(
                33120484,
                0,
                string.Empty,
                Enumeration.FromValue<UserLevel>(4),
                "master",
                "1234aa",
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                1000,
                1000000,
                33120480,
                "sourceUsername",
                "en-US",
                string.Empty,
                SystemIdentity.Chatbot,
                false);

            // Act
            param.SetUserName("masterZZ");

            // Assert
            Assert.Equal("masterZZ", param.UserName);
        }
    }
}