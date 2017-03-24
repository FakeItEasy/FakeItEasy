namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class FakeTTests
    {
        public static IEnumerable<object[]> CallSpecificationActions =>
            TestCases.FromObject<Action<Fake<IFoo>>>(
                fake => fake.CallsTo(foo => foo.Bar()),
                fake => fake.CallsTo(foo => foo.Baz()),
                fake => fake.AnyCall());

        [Fact]
        public void Constructor_that_takes_options_builder_should_be_null_guarded()
        {
            Action<IFakeOptions<Foo>> optionsBuilder = x => { };

            Expression<Action> call = () =>
                new Fake<Foo>(optionsBuilder);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void RecordedCalls_returns_recorded_calls_from_manager()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeManager(fake.FakedObject);

            fake.FakedObject.Bar();

            fake.RecordedCalls.Should().BeEquivalentTo(fakeObject.GetRecordedCalls());
        }

        [Theory]
        [MemberData(nameof(CallSpecificationActions))]
        public void Call_specifications_should_not_add_rule_to_manager(Action<Fake<IFoo>> action)
        {
            // Arrange
            var fake = new Fake<IFoo>();
            var manager = Fake.GetFakeManager(fake.FakedObject);
            var initialRules = manager.Rules.ToList();

            // Act
            action(fake);

            // Assert
            manager.Rules.Should().Equal(initialRules);
        }
    }
}
