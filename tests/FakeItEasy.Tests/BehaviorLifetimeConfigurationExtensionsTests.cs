namespace FakeItEasy.Tests;

using System;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FluentAssertions;
using Xunit;

public class BehaviorLifetimeConfigurationExtensionsTests
{
    [Fact]
    public void Once_should_call_NumberOfTimes_with_1_as_argument()
    {
        // Arrange
        var behaviorLifetimeConfiguration = A.Fake<IBehaviorLifetimeConfiguration<IVoidConfiguration>>();

        // Act
        behaviorLifetimeConfiguration.Once();

        // Assert
        A.CallTo(() => behaviorLifetimeConfiguration.NumberOfTimes(1)).MustHaveHappened();
    }

    [Fact]
    public void Once_should_be_null_guarded()
    {
        // Arrange

        // Act

        // Assert
        Expression<Action> call = () => A.Fake<IBehaviorLifetimeConfiguration<IVoidConfiguration>>().Once();
        call.Should().BeNullGuarded();
    }

    [Fact]
    public void Twice_should_call_NumberOfTimes_with_2_as_argument()
    {
        // Arrange
        var behaviorLifetimeConfiguration = A.Fake<IBehaviorLifetimeConfiguration<IVoidConfiguration>>();

        // Act
        behaviorLifetimeConfiguration.Twice();

        // Assert
        A.CallTo(() => behaviorLifetimeConfiguration.NumberOfTimes(2)).MustHaveHappened();
    }

    [Fact]
    public void Twice_should_be_null_guarded()
    {
        // Arrange

        // Act

        // Assert
        Expression<Action> call = () => A.Fake<IBehaviorLifetimeConfiguration<IVoidConfiguration>>().Twice();
        call.Should().BeNullGuarded();
    }
}
