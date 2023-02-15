namespace Alpha.CloneAccount.Tests.Utils
{
    using CloneAccount.Utils;
    using Xunit;

    [Trait("Category", "Utils")]
    public class LanguageUtilsTests
    {
        [Theory]
        [InlineData(null, "E")]
        [InlineData("", "E")]
        [InlineData("   ", "E")]
        [InlineData("en-US", "E")]
        [InlineData("ja-JP", "J")]
        [InlineData("zh-TW", "CT")]
        [InlineData("zh-CN", "CS")]
        [InlineData("th-TH", "TH")]
        [InlineData("ko-KR", "KO")]
        [InlineData("vi-VN", "VI")]
        [InlineData("abc", "E")]
        public void ToLanguageKey_Always_ReturnsCorrectToLanguageKey(string language, string languageKey)
        {
            // Arrange & Act
            var actualResult = LanguageUtils.ToLanguageKey(language);

            // Assert
            Assert.Equal(languageKey, actualResult);
        }
    }
}
