namespace Alpha.CloneAccount.Tests.Enumerations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CloneAccount.Enumerations;
    using CloneAccount.Models;
    using Models;
    using Xunit;

    [Trait("Category", "Enumerations")]
    public class EnumerationTests
    {
        [Theory]
        [InlineData(4, 4, 0)]
        [InlineData(4, 3, 1)]
        [InlineData(4, 2, 2)]
        [InlineData(4, 1, 3)]
        public void AbsoluteDifference_Always_ReturnsCorrectDifference(
            byte firstValue,
            byte secondValue,
            byte expectedDifference)
        {
            // Arrange
            var firstEnum = Enumeration.FromValue<UserLevel>(firstValue);
            var secondEnum = Enumeration.FromValue<UserLevel>(secondValue);

            // Act
            var actualDifference = Enumeration.AbsoluteDifference(firstEnum, secondEnum);

            // Assert
            Assert.Equal(expectedDifference, actualDifference);
        }

        [Theory]
        [InlineData("Super", 4)]
        [InlineData("Master", 3)]
        [InlineData("Agent", 2)]
        [InlineData("Member", 1)]
        public void FromDisplayName_Always_ReturnsExpectedValue(string displayName, int expectedValue)
        {
            // Act
            var actualValue = Enumeration.FromDisplayName<UserLevel>(displayName).Value;

            // Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Equal_CompareNullAndNotNullValue_ReturnsFalse()
        {
            // Arrange
            UserLevel left = null;
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actual = left == right;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void GetAll_GetAllUserLevels_ReturnsAllUserLevels()
        {
            // Arrange
            var expectedUserLevels = new List<UserLevel>()
            {
                Enumeration.FromValue<UserLevel>(4),
                Enumeration.FromValue<UserLevel>(3),
                Enumeration.FromValue<UserLevel>(2),
                Enumeration.FromValue<UserLevel>(2)
            };

            // Act
            var actualValues = Enumeration.GetAll(typeof(UserLevel)).Cast<UserLevel>();

            // Assert
            Assert.True(actualValues.Count() == expectedUserLevels.Count &&
                expectedUserLevels.FirstOrDefault(userLevel1 => actualValues.FirstOrDefault(userLevel2 => userLevel2.Value == userLevel1.Value) == null) == null);
        }

        [Fact]
        public void GreaterThan_NullLeftEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = null;
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left > right);
        }

        [Fact]
        public void GreaterThan_NullRightEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left > right);
        }

        [Fact]
        public void GreaterThan_LeftGreaterThanRight_ReturnsTrue()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(2);
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actualGreaterThan = left > right;

            // Assert
            Assert.True(actualGreaterThan);
        }

        [Fact]
        public void GreaterThan_LeftNotGreaterThanRight_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(2);

            // Act
            var actualGreaterThan = left > right;

            // Assert
            Assert.False(actualGreaterThan);
        }

        [Fact]
        public void NotLessThan_NullLeftEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = null;
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left >= right);
        }

        [Fact]
        public void NotLessThan_NullRightEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left >= right);
        }

        [Fact]
        public void NotLessThan_LeftNotLessThanRight_ReturnsTrue()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actualGreaterThan = left >= right;

            // Assert
            Assert.True(actualGreaterThan);
        }

        [Fact]
        public void NotLessThan_LeftLessThanRight_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(2);

            // Act
            var actualGreaterThan = left >= right;

            // Assert
            Assert.False(actualGreaterThan);
        }

        [Fact]
        public void LessThan_NullLeftEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = null;
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left < right);
        }

        [Fact]
        public void LessThan_NullRightEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left < right);
        }

        [Fact]
        public void LessThan_LeftLessThanRight_ReturnsTrue()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(2);

            // Act
            var actualGreaterThan = left < right;

            // Assert
            Assert.True(actualGreaterThan);
        }

        [Fact]
        public void LessThan_LeftNotLessThanRight_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actualGreaterThan = left < right;

            // Assert
            Assert.False(actualGreaterThan);
        }

        [Fact]
        public void NotGreaterThan_NullLeftEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = null;
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left <= right);
        }

        [Fact]
        public void NotGreaterThan_NullRightEnumeration_ThrowsArgumentNullException()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left <= right);
        }

        [Fact]
        public void NotGreaterThan_LeftNotGreaterThanRight_ReturnsTrue()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actualGreaterThan = left <= right;

            // Assert
            Assert.True(actualGreaterThan);
        }

        [Fact]
        public void NotGreaterThan_LeftGreaterThanRight_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(2);
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actualGreaterThan = left <= right;

            // Assert
            Assert.False(actualGreaterThan);
        }

        [Fact]
        public void Equals_CompareWithNull_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = null;

            // Act
            var actual = left.Equals(right);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_CompareWithOtherEnumeration_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            var right = 2;

            // Act
            var actual = left.Equals(right);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_CompareWithSubEnumeration_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevelForTesting right = Enumeration.FromValue<UserLevelForTesting>(0);

            // Act
            var actual = left.Equals(right);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_CompareWithDifferentValue_ReturnsFalse()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(2);

            // Act
            var actual = left.Equals(right);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_CompareWithSameEnumerationValue_ReturnsTrue()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = Enumeration.FromValue<UserLevel>(1);

            // Act
            var actual = left.Equals(right);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void GetHashCode_Always_ReturnsHashCode()
        {
            // Arrange
            UserLevel value = Enumeration.FromValue<UserLevel>(1);
            var expectedHashCode = 1;

            // Act
            var actual = value.GetHashCode();

            // Assert
            Assert.Equal(expectedHashCode, actual);
        }

        [Theory]
        [InlineData(2, 2, 0)]
        [InlineData(2, 3, -1)]
        [InlineData(2, 1, 1)]
        public void CompareTo_CompareWithNotNullValue_ReturnsExpectedCompareValue(
            byte leftValue,
            byte rightValue,
            int expectedCompareValue)
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(leftValue);
            UserLevel right = Enumeration.FromValue<UserLevel>(rightValue);

            // Act
            var actual = left.CompareTo(right);

            // Assert
            Assert.Equal(expectedCompareValue, actual);
        }

        [Fact]
        public void CompareTo_CompareWithNullValue_ReturnsExpectedCompareValue()
        {
            // Arrange
            UserLevel left = Enumeration.FromValue<UserLevel>(1);
            UserLevel right = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => left.CompareTo(right));
        }

        [Fact]
        public void Parse_NoMatchingItem_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            byte noMatchingValue = 0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Enumeration.FromValue<UserLevel>(noMatchingValue));
        }

        [Theory]
        [InlineData("Member")]
        [InlineData("Agent")]
        [InlineData("Master")]
        [InlineData("Super")]
        public void ToString_Always_ReturnsDisplayName(string displayName)
        {
            // Arrange
            UserLevel value = Enumeration.FromDisplayName<UserLevel>(displayName);

            // Act
            var actual = value.ToString();

            // Assert
            Assert.Equal(displayName, actual);
        }
    }
}
