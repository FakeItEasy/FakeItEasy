namespace FakeItEasy.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using Xunit;

public class ACallToTests
{
    public static IEnumerable<object?[]> CallSpecificationActions =>
        TestCases.FromObject<Action<IFoo>>(
            foo => A.CallTo(() => foo.Bar()),
            foo => A.CallTo(() => foo.Baz()),
            foo => A.CallToSet(() => foo.SomeProperty),
            foo => A.CallTo(foo));

    [Theory]
    [MemberData(nameof(CallSpecificationActions))]
    public void CallTo_should_not_add_rule_to_manager(Action<IFoo> action)
    {
        // Arrange
        var foo = A.Fake<IFoo>();
        var manager = Fake.GetFakeManager(foo);
        var initialRules = manager.Rules.ToList();

        // Act
        action(foo);

        // Assert
        manager.Rules.Should().Equal(initialRules);
    }

    [Fact]
    public void CallToSet_should_be_null_guarded()
    {
        // Arrange
        var foo = A.Fake<IFoo>();
        Expression<Action> call = () => A.CallToSet(() => foo.SomeProperty);

        // Act

        // Assert
        call.Should().BeNullGuarded();
    }
}
