namespace FakeItEasy.Tests;

using System;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FluentAssertions;
using Xunit;

public class ArgumentValidationConfigurationExtensionsTests
{
    [Fact]
    public void WithAnyArguments_with_void_call_should_call_when_arguments_match_with_predicate_that_returns_true()
    {
        // Arrange
        var configuration = A.Fake<IArgumentValidationConfiguration<IVoidConfiguration>>();

        // Act
        configuration.WithAnyArguments();

        // Assert
        var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection?, bool>>(0)!;

        predicate.Invoke(null).Should().BeTrue();
    }

    [Fact]
    public void WithAnyArguments_with_function_call_should_call_when_arguments_match_with_predicate_that_returns_true()
    {
        // Arrange
        var configuration = A.Fake<IArgumentValidationConfiguration<IReturnValueConfiguration<int>>>();

        // Act
        configuration.WithAnyArguments();

        // Assert
        var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection?, bool>>(0)!;

        predicate.Invoke(null).Should().BeTrue();
    }

    [Fact]
    public void WithAnyArguments_should_be_null_guarded()
    {
        // Arrange

        // Act

        // Assert
        Expression<Action> call = () => A.Fake<IArgumentValidationConfiguration<IVoidConfiguration>>().WithAnyArguments();
        call.Should().BeNullGuarded();
    }
}
