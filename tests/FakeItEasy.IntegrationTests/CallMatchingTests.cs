namespace FakeItEasy.IntegrationTests;

using System;
using FluentAssertions;
using Xunit;

public class CallMatchingTests
{
    [Fact]
    public void Comparing_string_arguments_does_not_use_general_enumerable_comparer()
    {
        // Arrange
        var fake = A.Fake<Action<string>>();

        A.CallTo(() => fake.Invoke("abc")).DoesNothing();

        // Act
        var exception = Record.Exception(() => fake("abc"));

        // Assert
        exception.Should().BeNull();
    }

    public class ThrowingCharComparer : ArgumentEqualityComparer<char>
    {
        protected override bool AreEqual(char expectedValue, char argumentValue)
            => throw new InvalidOperationException("We don't want to compare chars individually");
    }
}
